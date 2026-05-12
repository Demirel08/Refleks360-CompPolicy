using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Abstractions;
using Refleks360.Application.PayEquity;
using Refleks360.Domain.Organization;
using Refleks360.Infrastructure.Persistence;

namespace Refleks360.Infrastructure.Services;

public sealed class PayEquityService(CompDbContext db) : IPayEquityService
{
    public async Task<PayEquityResult> ComputeAsync(CancellationToken ct = default)
    {
        var rows = await db.Employees.AsNoTracking()
            .Where(e => e.Status == EmployeeStatus.Active)
            .Select(e => new
            {
                e.Gender,
                e.Position.JobGradeId,
                GradeCode = e.Position.JobGrade.Code,
                Department = e.Department.Name,
                Gross = db.EmployeeSalaries
                    .Where(s => s.EmployeeId == e.Id && s.EndDate == null)
                    .Select(s => (decimal?)s.GrossMonthly).FirstOrDefault(),
            })
            .ToListAsync(ct);

        var withSalary = rows.Where(r => r.Gross.HasValue).ToList();

        var males = withSalary.Where(r => r.Gender == Gender.Male).ToList();
        var females = withSalary.Where(r => r.Gender == Gender.Female).ToList();
        decimal mAvg = males.Count > 0 ? Math.Round(males.Average(r => r.Gross!.Value), 2) : 0m;
        decimal fAvg = females.Count > 0 ? Math.Round(females.Average(r => r.Gross!.Value), 2) : 0m;
        decimal rawGap = mAvg > 0 ? Math.Round((1m - fAvg / mAvg) * 100m, 2) : 0m;

        // Düzeltilmiş gap (basit yaklaşım): kademe başına ortalamaları al,
        // toplam çalışan ağırlıklı ortalama farkı hesapla.
        decimal weightedDiff = 0m;
        decimal totalWeight = 0m;
        foreach (var g in withSalary.GroupBy(r => r.JobGradeId))
        {
            var m = g.Where(r => r.Gender == Gender.Male).ToList();
            var f = g.Where(r => r.Gender == Gender.Female).ToList();
            if (m.Count == 0 || f.Count == 0) continue;
            decimal mA = m.Average(r => r.Gross!.Value);
            decimal fA = f.Average(r => r.Gross!.Value);
            if (mA <= 0m) continue;
            decimal gap = 1m - fA / mA;
            decimal w = m.Count + f.Count;
            weightedDiff += gap * w;
            totalWeight += w;
        }
        decimal adjustedGap = totalWeight > 0 ? Math.Round(weightedDiff / totalWeight * 100m, 2) : 0m;

        // Cohort by grade
        var byGrade = withSalary.GroupBy(r => r.GradeCode).Select(g =>
        {
            var m = g.Where(r => r.Gender == Gender.Male).ToList();
            var f = g.Where(r => r.Gender == Gender.Female).ToList();
            decimal mavg = m.Count > 0 ? Math.Round(m.Average(r => r.Gross!.Value), 0) : 0m;
            decimal favg = f.Count > 0 ? Math.Round(f.Average(r => r.Gross!.Value), 0) : 0m;
            decimal gap = mavg > 0 ? Math.Round((1m - favg / mavg) * 100m, 1) : 0m;
            return new PayEquityCohort(g.Key, m.Count, mavg, f.Count, favg, gap);
        }).OrderBy(c => c.Label).ToList();

        var byDept = withSalary.GroupBy(r => r.Department).Select(g =>
        {
            var m = g.Where(r => r.Gender == Gender.Male).ToList();
            var f = g.Where(r => r.Gender == Gender.Female).ToList();
            decimal mavg = m.Count > 0 ? Math.Round(m.Average(r => r.Gross!.Value), 0) : 0m;
            decimal favg = f.Count > 0 ? Math.Round(f.Average(r => r.Gross!.Value), 0) : 0m;
            decimal gap = mavg > 0 ? Math.Round((1m - favg / mavg) * 100m, 1) : 0m;
            return new PayEquityCohort(g.Key, m.Count, mavg, f.Count, favg, gap);
        }).OrderBy(c => c.Label).ToList();

        return new PayEquityResult(mAvg, fAvg, rawGap, adjustedGap, byGrade, byDept);
    }
}
