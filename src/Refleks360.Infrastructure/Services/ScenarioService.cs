using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Abstractions;
using Refleks360.Application.Scenarios;
using Refleks360.Domain.Calculations;
using Refleks360.Domain.Organization;
using Refleks360.Domain.Scenarios;
using Refleks360.Infrastructure.Persistence;
using Refleks360.Infrastructure.Persistence.Entities;

namespace Refleks360.Infrastructure.Services;

public sealed class ScenarioService(CompDbContext db, ITaxParameterService taxParams) : IScenarioService
{
    public async Task<IReadOnlyList<ScenarioListItem>> GetAllAsync(CancellationToken ct = default)
    {
        return await db.Scenarios.AsNoTracking()
            .OrderByDescending(s => s.CreatedAtUtc)
            .Select(s => new ScenarioListItem(
                s.Id, s.Name, s.Type, s.Status, s.BaseDate, s.EffectiveDate,
                s.Employees.Count,
                s.Employees.Sum(e => e.OldGross),
                s.Employees.Sum(e => e.NewGross),
                s.Employees.Sum(e => e.OldEmployerCost),
                s.Employees.Sum(e => e.NewEmployerCost),
                s.CreatedBy, s.CreatedAtUtc))
            .ToListAsync(ct);
    }

    public async Task<ScenarioDetail?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var sc = await db.Scenarios.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, ct);
        if (sc is null) return null;

        var employees = await db.ScenarioEmployees.AsNoTracking()
            .Where(e => e.ScenarioId == id)
            .Select(e => new ScenarioEmployeeRow(
                e.Id, e.EmployeeId,
                e.Employee.EmployeeNumber,
                e.Employee.FirstName + " " + e.Employee.LastName,
                e.Employee.Department.Name,
                e.Employee.Position.Title,
                e.OldGross, e.NewGross,
                e.OldNetMonthly, e.NewNetMonthly,
                e.OldEmployerCost, e.NewEmployerCost,
                e.RaisePercent, e.RaiseAmount,
                e.IsLocked))
            .OrderBy(e => e.FullName)
            .ToListAsync(ct);

        return new ScenarioDetail(
            sc.Id, sc.Name, sc.Description, sc.Type, sc.Status,
            sc.BaseDate, sc.EffectiveDate, sc.ParametersJson,
            sc.CreatedBy, sc.CreatedAtUtc, sc.AppliedAtUtc,
            employees);
    }

    public async Task<int> CreateAsync(CreateScenarioInput input, string createdBy, CancellationToken ct = default)
    {
        // 1) Çalışan listesi
        var query = db.Employees.AsNoTracking()
            .Where(e => e.Status == EmployeeStatus.Active);
        if (input.IncludedEmployeeIds is { Count: > 0 })
        {
            var ids = input.IncludedEmployeeIds.ToHashSet();
            query = query.Where(e => ids.Contains(e.Id));
        }
        var employees = await query
            .Select(e => new
            {
                e.Id, e.DepartmentId, e.PositionId, e.LocationId,
                CurrentGross = db.EmployeeSalaries
                    .Where(s => s.EmployeeId == e.Id && s.EndDate == null)
                    .Select(s => (decimal?)s.GrossMonthly).FirstOrDefault(),
            })
            .ToListAsync(ct);

        // Maaşı olmayanları atla
        employees = employees.Where(e => e.CurrentGross.HasValue && e.CurrentGross > 0m).ToList();

        // 2) Yeni gross hesabı
        var lockedSet = (input.LockedEmployeeIds ?? Array.Empty<int>()).ToHashSet();
        var minWage = input.MinWageFloor ? (input.MinWageGross ?? 0m) : 0m;
        decimal minGuaranteed = input.MinGuaranteedPercent ?? 0m;

        // Targeted budget hesabı: önce locked + guaranteed dağıt, sonra kalan bütçe için ortak oran
        // Basitleştirilmiş Mod A (oransal) + Mod B (min garantili) algoritması.
        var raiseByEmp = new Dictionary<int, (decimal newGross, decimal pct, bool locked)>();

        decimal totalAnnualCurrent = employees.Sum(e => e.CurrentGross!.Value) * 12m;

        switch (input.Type)
        {
            case ScenarioType.GeneralRaise:
            {
                decimal pct = input.GeneralRaisePercent ?? 0m;
                foreach (var emp in employees)
                {
                    decimal newGross;
                    decimal usedPct = pct;
                    bool locked = lockedSet.Contains(emp.Id);
                    if (locked && input.LockedEmployeePercents?.TryGetValue(emp.Id, out var lp) == true) usedPct = lp;
                    newGross = Math.Round(emp.CurrentGross!.Value * (1m + usedPct / 100m), 2);
                    if (minWage > 0m && newGross < minWage) newGross = minWage;
                    raiseByEmp[emp.Id] = (newGross, ((newGross / emp.CurrentGross.Value) - 1m) * 100m, locked);
                }
                break;
            }
            case ScenarioType.DepartmentRaise:
            {
                var rates = input.DepartmentRaiseRates ?? new();
                foreach (var emp in employees)
                {
                    decimal usedPct = lockedSet.Contains(emp.Id) && input.LockedEmployeePercents?.TryGetValue(emp.Id, out var lp) == true
                        ? lp
                        : (rates.TryGetValue(emp.DepartmentId, out var dp) ? dp : 0m);
                    decimal newGross = Math.Round(emp.CurrentGross!.Value * (1m + usedPct / 100m), 2);
                    if (minWage > 0m && newGross < minWage) newGross = minWage;
                    raiseByEmp[emp.Id] = (newGross, ((newGross / emp.CurrentGross.Value) - 1m) * 100m, lockedSet.Contains(emp.Id));
                }
                break;
            }
            case ScenarioType.PositionRaise:
            {
                var rates = input.PositionRaiseRates ?? new();
                foreach (var emp in employees)
                {
                    decimal usedPct = lockedSet.Contains(emp.Id) && input.LockedEmployeePercents?.TryGetValue(emp.Id, out var lp) == true
                        ? lp
                        : (rates.TryGetValue(emp.PositionId, out var pp) ? pp : 0m);
                    decimal newGross = Math.Round(emp.CurrentGross!.Value * (1m + usedPct / 100m), 2);
                    if (minWage > 0m && newGross < minWage) newGross = minWage;
                    raiseByEmp[emp.Id] = (newGross, ((newGross / emp.CurrentGross.Value) - 1m) * 100m, lockedSet.Contains(emp.Id));
                }
                break;
            }
            case ScenarioType.TargetedBudget:
            {
                decimal target = input.TargetTotalAnnualCost ?? 0m;

                // Önce herkese min garantiyi uygula
                decimal allocatedAnnual = 0m;
                var temp = new Dictionary<int, decimal>();
                foreach (var emp in employees)
                {
                    decimal pct;
                    if (lockedSet.Contains(emp.Id) && input.LockedEmployeePercents?.TryGetValue(emp.Id, out var lp) == true)
                        pct = lp;
                    else
                        pct = minGuaranteed;

                    decimal newGross = Math.Round(emp.CurrentGross!.Value * (1m + pct / 100m), 2);
                    temp[emp.Id] = newGross;
                    allocatedAnnual += newGross * 12m;
                }
                // Kalan bütçeyi non-locked çalışanlara oransal dağıt
                decimal remainingAnnual = target - allocatedAnnual;
                if (remainingAnnual > 0m)
                {
                    var freeIds = employees.Where(e => !lockedSet.Contains(e.Id)).Select(e => e.Id).ToList();
                    decimal freeAnnualSum = freeIds.Sum(id => temp[id]) * 12m;
                    if (freeAnnualSum > 0m)
                    {
                        decimal extraFactor = remainingAnnual / freeAnnualSum; // ek oransal artış
                        foreach (var id in freeIds)
                        {
                            temp[id] = Math.Round(temp[id] * (1m + extraFactor), 2);
                        }
                    }
                }

                foreach (var emp in employees)
                {
                    decimal newGross = temp[emp.Id];
                    if (minWage > 0m && newGross < minWage) newGross = minWage;
                    raiseByEmp[emp.Id] = (newGross, ((newGross / emp.CurrentGross!.Value) - 1m) * 100m, lockedSet.Contains(emp.Id));
                }
                break;
            }
            default:
                throw new NotSupportedException($"Senaryo tipi desteklenmiyor: {input.Type}");
        }

        // 3) Net + işveren maliyeti hesabı (yıllık ortalama)
        var taxYear = await taxParams.GetForYearAsync(input.BaseDate.Year, ct);
        var paramsForCalc = taxYear.Parameters;
        var periodsByMonth = taxYear.PeriodsByMonth;

        decimal AvgAnnualEmployerCost(decimal gross)
        {
            decimal cum = 0m, total = 0m;
            for (int m = 1; m <= 12; m++)
            {
                var period = periodsByMonth[m];
                var res = SalaryCalculator.CalculateMonth(gross, cum, period, paramsForCalc);
                total += res.EmployerCost;
                cum += res.MonthlyTaxBase;
            }
            return Math.Round(total / 12m, 2);
        }

        decimal AvgAnnualNet(decimal gross)
        {
            decimal cum = 0m, total = 0m;
            for (int m = 1; m <= 12; m++)
            {
                var period = periodsByMonth[m];
                var res = SalaryCalculator.CalculateMonth(gross, cum, period, paramsForCalc);
                total += res.Net;
                cum += res.MonthlyTaxBase;
            }
            return Math.Round(total / 12m, 2);
        }

        // 4) Scenario + ScenarioEmployee'leri DB'ye yaz
        var scenario = new ScenarioEntity
        {
            Name = input.Name,
            Description = input.Description,
            BaseDate = input.BaseDate,
            EffectiveDate = input.EffectiveDate,
            Type = input.Type,
            Status = ScenarioStatus.Calculated,
            ParametersJson = JsonSerializer.Serialize(input),
            CreatedBy = createdBy,
        };
        db.Scenarios.Add(scenario);
        await db.SaveChangesAsync(ct);

        var scRows = new List<ScenarioEmployeeEntity>(employees.Count);
        foreach (var emp in employees)
        {
            decimal oldGross = emp.CurrentGross!.Value;
            var r = raiseByEmp[emp.Id];
            decimal newGross = r.newGross;
            scRows.Add(new ScenarioEmployeeEntity
            {
                ScenarioId = scenario.Id,
                EmployeeId = emp.Id,
                OldGross = oldGross,
                NewGross = newGross,
                OldNetMonthly = AvgAnnualNet(oldGross),
                NewNetMonthly = AvgAnnualNet(newGross),
                OldEmployerCost = AvgAnnualEmployerCost(oldGross),
                NewEmployerCost = AvgAnnualEmployerCost(newGross),
                RaisePercent = Math.Round(r.pct, 4),
                RaiseAmount = Math.Round(newGross - oldGross, 2),
                IsLocked = r.locked,
            });
        }
        db.ScenarioEmployees.AddRange(scRows);
        await db.SaveChangesAsync(ct);

        return scenario.Id;
    }

    public async Task ApplyAsync(int scenarioId, string appliedBy, CancellationToken ct = default)
    {
        var sc = await db.Scenarios.FirstAsync(s => s.Id == scenarioId, ct);
        if (sc.Status == ScenarioStatus.Applied)
            throw new InvalidOperationException("Bu senaryo zaten uygulandı.");

        var rows = await db.ScenarioEmployees.Where(e => e.ScenarioId == scenarioId).ToListAsync(ct);
        var effDate = sc.EffectiveDate ?? DateOnly.FromDateTime(DateTime.Today);

        foreach (var r in rows)
        {
            // Mevcut açık ücret satırını kapat
            var current = await db.EmployeeSalaries
                .Where(s => s.EmployeeId == r.EmployeeId && s.EndDate == null)
                .OrderByDescending(s => s.EffectiveDate)
                .FirstOrDefaultAsync(ct);
            if (current is not null)
            {
                current.EndDate = effDate.AddDays(-1);
            }

            db.EmployeeSalaries.Add(new EmployeeSalaryEntity
            {
                EmployeeId = r.EmployeeId,
                GrossMonthly = r.NewGross,
                NetMonthlyCached = r.NewNetMonthly,
                EmployerCostCached = r.NewEmployerCost,
                EffectiveDate = effDate,
                Reason = SalaryChangeReason.AnnualMerit,
                ScenarioId = scenarioId,
                ChangePercent = r.RaisePercent,
                ChangeAmount = r.RaiseAmount,
                CreatedBy = appliedBy,
            });
        }

        sc.Status = ScenarioStatus.Applied;
        sc.AppliedAtUtc = DateTime.UtcNow;
        sc.AppliedBy = appliedBy;
        await db.SaveChangesAsync(ct);
    }

    public async Task CancelAsync(int scenarioId, CancellationToken ct = default)
    {
        var sc = await db.Scenarios.FirstAsync(s => s.Id == scenarioId, ct);
        if (sc.Status == ScenarioStatus.Applied)
            throw new InvalidOperationException("Uygulanmış senaryo iptal edilemez (geri alma için ayrı bir akış gerekir).");
        sc.Status = ScenarioStatus.Cancelled;
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int scenarioId, CancellationToken ct = default)
    {
        var sc = await db.Scenarios.FirstAsync(s => s.Id == scenarioId, ct);
        if (sc.Status == ScenarioStatus.Applied)
            throw new InvalidOperationException("Uygulanmış senaryo silinemez.");
        db.Scenarios.Remove(sc);
        await db.SaveChangesAsync(ct);
    }
}
