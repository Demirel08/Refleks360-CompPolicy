using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Abstractions;
using Refleks360.Application.MultiYear;
using Refleks360.Domain.Calculations;
using Refleks360.Domain.Organization;
using Refleks360.Infrastructure.Persistence;

namespace Refleks360.Infrastructure.Services;

public sealed class MultiYearPlanService(CompDbContext db, ITaxParameterService taxParams) : IMultiYearPlanService
{
    public async Task<MultiYearPlan> ProjectAsync(int years, decimal annualRaisePercent, decimal annualInflationPercent, CancellationToken ct = default)
    {
        var rows = await db.Employees.AsNoTracking()
            .Where(e => e.Status == EmployeeStatus.Active)
            .Select(e => new
            {
                Gross = db.EmployeeSalaries.Where(s => s.EmployeeId == e.Id && s.EndDate == null)
                    .Select(s => (decimal?)s.GrossMonthly).FirstOrDefault()
            })
            .ToListAsync(ct);

        var current = rows.Where(r => r.Gross.HasValue).Select(r => r.Gross!.Value).ToList();
        int headcount = current.Count;

        var taxYearNow = DateTime.UtcNow.Year;
        var tax = await taxParams.GetForYearAsync(taxYearNow, ct);
        decimal MonthlyCost(decimal gross)
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

        decimal totalMonthly = current.Sum(MonthlyCost);
        decimal currentAnnual = totalMonthly * 12m;

        var projections = new List<YearProjection>(years);
        decimal factor = 1m;
        for (int y = 1; y <= years; y++)
        {
            factor *= (1m + annualRaisePercent / 100m);
            decimal projectedMonthly = totalMonthly * factor;
            decimal projectedAnnual = projectedMonthly * 12m;
            decimal projectedPayroll = current.Sum() * factor * 12m;
            projections.Add(new YearProjection(taxYearNow + y, Math.Round(projectedMonthly, 0), Math.Round(projectedAnnual, 0), Math.Round(projectedPayroll, 0)));
        }

        return new MultiYearPlan(Math.Round(current.Sum() * 12m, 0), projections, annualInflationPercent, annualRaisePercent, headcount);
    }

    public async Task<DepartmentGradeHeatMap> ComputeDeptGradeHeatMapAsync(CancellationToken ct = default)
    {
        var rows = await db.Employees.AsNoTracking()
            .Where(e => e.Status == EmployeeStatus.Active)
            .Select(e => new
            {
                Department = e.Department.Name,
                GradeCode = e.Position.JobGrade.Code,
                JobGradeId = e.Position.JobGradeId,
                LocationId = e.LocationId,
                Gross = db.EmployeeSalaries.Where(s => s.EmployeeId == e.Id && s.EndDate == null)
                    .Select(s => (decimal?)s.GrossMonthly).FirstOrDefault()
            })
            .ToListAsync(ct);

        var bands = await db.SalaryBands.AsNoTracking().Where(b => b.EndDate == null).ToListAsync(ct);
        var bandByGradeLoc = bands.ToDictionary(b => (b.JobGradeId, b.LocationId), b => b);

        var depts = rows.Select(r => r.Department).Distinct().OrderBy(d => d).ToList();
        var grades = rows.Select(r => r.GradeCode).Distinct().OrderBy(g => g).ToList();
        var cells = new List<DepartmentGradeHeatCell>();

        foreach (var d in depts)
        {
            foreach (var g in grades)
            {
                var subset = rows.Where(r => r.Department == d && r.GradeCode == g && r.Gross.HasValue).ToList();
                int hc = subset.Count;
                decimal? avgCompa = null;
                if (hc > 0)
                {
                    decimal sum = 0m; int cnt = 0;
                    foreach (var s in subset)
                    {
                        var band = bandByGradeLoc.TryGetValue((s.JobGradeId, s.LocationId), out var b1) ? b1
                                 : bandByGradeLoc.TryGetValue((s.JobGradeId, null), out var b2) ? b2 : null;
                        if (band is not null && band.Mid > 0m) { sum += s.Gross!.Value / band.Mid; cnt++; }
                    }
                    if (cnt > 0) avgCompa = Math.Round(sum / cnt, 2);
                }
                cells.Add(new DepartmentGradeHeatCell(d, g, hc, avgCompa));
            }
        }

        return new DepartmentGradeHeatMap(grades, depts, cells);
    }
}
