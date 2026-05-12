using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Abstractions;
using Refleks360.Application.Dashboards;
using Refleks360.Domain.Calculations;
using Refleks360.Domain.Organization;
using Refleks360.Domain.Scenarios;
using Refleks360.Infrastructure.Persistence;

namespace Refleks360.Infrastructure.Services;

public sealed class DashboardService(CompDbContext db, ITaxParameterService taxParams) : IDashboardService
{
    public async Task<DashboardSummary> GetSummaryAsync(CancellationToken ct = default)
    {
        var total = await db.Employees.CountAsync(ct);
        var active = await db.Employees.CountAsync(e => e.Status == EmployeeStatus.Active, ct);

        var rows = await db.Employees.AsNoTracking()
            .Select(e => new
            {
                e.Id, e.Status, e.Gender, e.LocationId,
                JobGradeId = e.Position.JobGradeId,
                Department = e.Department.Name,
                CurrentGross = db.EmployeeSalaries
                    .Where(s => s.EmployeeId == e.Id && s.EndDate == null)
                    .Select(s => (decimal?)s.GrossMonthly).FirstOrDefault(),
            })
            .ToListAsync(ct);

        var bands = await db.SalaryBands.AsNoTracking()
            .Where(b => b.EndDate == null)
            .ToListAsync(ct);
        var bandByGradeLoc = bands.ToDictionary(b => (b.JobGradeId, b.LocationId), b => b);

        // Yıllık ortalama işveren maliyeti için hesap motoru
        var year = DateTime.UtcNow.Year;
        var tax = await taxParams.GetForYearAsync(year, ct);
        decimal MonthlyEmployerCost(decimal gross)
        {
            decimal cum = 0m, total = 0m;
            for (int m = 1; m <= 12; m++)
            {
                var p = tax.PeriodsByMonth[m];
                var r = SalaryCalculator.CalculateMonth(gross, cum, p, tax.Parameters);
                total += r.EmployerCost;
                cum += r.MonthlyTaxBase;
            }
            return total / 12m;
        }

        decimal compaSum = 0m; int compaCount = 0;
        decimal monthlyEmployerCostTotal = 0m;
        var deptCost = new Dictionary<string, (decimal Cost, int Heads)>();

        decimal sumMale = 0m, sumFemale = 0m;
        int countMale = 0, countFemale = 0;

        foreach (var r in rows)
        {
            decimal gross = r.CurrentGross ?? 0m;
            if (gross > 0m && r.Status == EmployeeStatus.Active)
            {
                var mec = MonthlyEmployerCost(gross);
                monthlyEmployerCostTotal += mec;
                if (!deptCost.TryAdd(r.Department, (mec, 1)))
                {
                    var existing = deptCost[r.Department];
                    deptCost[r.Department] = (existing.Cost + mec, existing.Heads + 1);
                }

                var band = bandByGradeLoc.TryGetValue((r.JobGradeId, r.LocationId), out var b1) ? b1
                         : bandByGradeLoc.TryGetValue((r.JobGradeId, null), out var b2) ? b2 : null;
                if (band is not null && band.Mid > 0m)
                {
                    compaSum += gross / band.Mid;
                    compaCount++;
                }

                if (r.Gender == Gender.Male) { sumMale += gross; countMale++; }
                else if (r.Gender == Gender.Female) { sumFemale += gross; countFemale++; }
            }
        }

        decimal avgCompa = compaCount > 0 ? Math.Round(compaSum / compaCount, 3) : 0m;
        decimal avgMale = countMale > 0 ? Math.Round(sumMale / countMale, 2) : 0m;
        decimal avgFemale = countFemale > 0 ? Math.Round(sumFemale / countFemale, 2) : 0m;
        decimal rawGap = avgMale > 0 ? Math.Round((1m - avgFemale / avgMale) * 100m, 2) : 0m;

        var pendingScenarios = await db.Scenarios.CountAsync(s => s.Status == ScenarioStatus.Calculated, ct);

        var cost = deptCost.Select(kv => new DepartmentCostSlice(kv.Key, Math.Round(kv.Value.Cost, 0), kv.Value.Heads))
            .OrderByDescending(x => x.MonthlyEmployerCost).ToList();

        return new DashboardSummary(
            total, active,
            Math.Round(monthlyEmployerCostTotal, 0),
            avgCompa,
            pendingScenarios,
            new GenderPayGap(avgMale, avgFemale, rawGap),
            cost);
    }
}
