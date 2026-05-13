using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Refleks360.Application.Abstractions;
using Refleks360.Application.Payroll;
using Refleks360.Domain.Calculations;
using Refleks360.Domain.Organization;
using Refleks360.Infrastructure.Persistence;

namespace Refleks360.Infrastructure.Services;

public sealed class PayrollViewService(
    IDbContextFactory<CompDbContext> dbFactory,
    ITaxParameterService taxParams,
    IMemoryCache cache) : IPayrollViewService
{
    private const string CacheKey = "payroll-snapshot";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(30);

    public async Task<PayrollSnapshot> GetCurrentSnapshotAsync(CancellationToken ct = default)
    {
        if (cache.TryGetValue<PayrollSnapshot>(CacheKey, out var cached) && cached is not null)
            return cached;

        await using var db = await dbFactory.CreateDbContextAsync(ct);

        var people = await db.Employees.AsNoTracking()
            .Where(e => e.Status == EmployeeStatus.Active)
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .Select(e => new
            {
                e.Id,
                e.EmployeeNumber,
                FullName = e.FirstName + " " + e.LastName,
                Department = e.Department.Name,
                Position = e.Position.Title,
                JobGradeId = e.Position.JobGradeId,
                GradeCode = e.Position.JobGrade.Code,
                LocationId = e.LocationId,
            })
            .ToListAsync(ct);

        var currentSalaries = await db.EmployeeSalaries.AsNoTracking()
            .Where(s => s.EndDate == null)
            .Select(s => new { s.EmployeeId, s.GrossMonthly })
            .ToDictionaryAsync(s => s.EmployeeId, s => s.GrossMonthly, ct);

        var bands = await db.SalaryBands.AsNoTracking().Where(b => b.EndDate == null)
            .Select(b => new { b.JobGradeId, b.LocationId, b.Min, b.Mid, b.Max })
            .ToListAsync(ct);
        var bandByGradeLoc = bands.ToDictionary(b => (b.JobGradeId, b.LocationId), b => b);

        // Hesap motoru için aynı yıl içinde 12 ay simüle et, net + işveren maliyetini al
        var year = DateTime.UtcNow.Year;
        var tax = await taxParams.GetForYearAsync(year, ct);

        // Brut bazında memoize (aynı brut tekrar gelirse hesap yok)
        var memo = new Dictionary<decimal, (decimal Net, decimal EmpCost)>();
        (decimal net, decimal empCost) Calc(decimal gross)
        {
            if (memo.TryGetValue(gross, out var c)) return c;
            decimal cum = 0m, sumNet = 0m, sumCost = 0m;
            for (int m = 1; m <= 12; m++)
            {
                var p = tax.PeriodsByMonth[m];
                var r = SalaryCalculator.CalculateMonth(gross, cum, p, tax.Parameters);
                sumNet += r.Net;
                sumCost += r.EmployerCost;
                cum += r.MonthlyTaxBase;
            }
            var result = (Math.Round(sumNet / 12m, 2), Math.Round(sumCost / 12m, 2));
            memo[gross] = result;
            return result;
        }

        var rows = new List<EmployeePayrollRow>(people.Count);
        decimal totalGross = 0m, totalNet = 0m, totalCost = 0m;

        foreach (var p in people)
        {
            decimal? gross = currentSalaries.TryGetValue(p.Id, out var g) ? g : null;
            decimal? net = null, empCost = null, compa = null;
            string? flag = null;

            if (gross is { } g0 && g0 > 0m)
            {
                var calc = Calc(g0);
                net = calc.net;
                empCost = calc.empCost;
                totalGross += g0;
                totalNet += calc.net;
                totalCost += calc.empCost;

                var band = bandByGradeLoc.TryGetValue((p.JobGradeId, p.LocationId), out var b1) ? b1
                         : bandByGradeLoc.TryGetValue((p.JobGradeId, null), out var b2) ? b2 : null;
                if (band is not null && band.Mid > 0m && band.Max > band.Min)
                {
                    compa = Math.Round(g0 / band.Mid, 3);
                    flag = g0 < band.Min ? "below" : g0 > band.Max ? "above" : "in";
                }
            }

            rows.Add(new EmployeePayrollRow(
                p.Id, p.EmployeeNumber, p.FullName, p.Department, p.Position, p.GradeCode,
                gross, net, empCost, compa, flag));
        }

        var snapshot = new PayrollSnapshot(
            year, people.Count,
            Math.Round(totalGross, 0), Math.Round(totalNet, 0), Math.Round(totalCost, 0),
            rows);

        cache.Set(CacheKey, snapshot, CacheTtl);
        return snapshot;
    }
}
