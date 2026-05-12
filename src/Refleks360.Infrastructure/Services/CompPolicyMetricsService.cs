using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Abstractions;
using Refleks360.Application.CompPolicy;
using Refleks360.Infrastructure.Persistence;

namespace Refleks360.Infrastructure.Services;

public sealed class CompPolicyMetricsService(CompDbContext db) : ICompPolicyMetricsService
{
    public async Task<CompPolicyOverview> ComputeOverviewAsync(CancellationToken ct = default)
    {
        var rows = await db.Employees.AsNoTracking()
            .Select(e => new
            {
                e.Id,
                e.EmployeeNumber,
                FullName = e.FirstName + " " + e.LastName,
                Department = e.Department.Name,
                JobGradeId = e.Position.JobGradeId,
                e.LocationId,
                Current = db.EmployeeSalaries
                    .Where(s => s.EmployeeId == e.Id && s.EndDate == null)
                    .Select(s => (decimal?)s.GrossMonthly).FirstOrDefault(),
            })
            .ToListAsync(ct);

        var bands = await db.SalaryBands.AsNoTracking()
            .Where(b => b.EndDate == null)
            .ToListAsync(ct);
        var bandByGradeLoc = bands.ToDictionary(b => (b.JobGradeId, b.LocationId), b => b);

        var stats = new List<(int Id, string No, string Name, string Dept, decimal Gross, decimal Min, decimal Mid, decimal Max, decimal Compa, decimal Pen, string Flag)>();
        foreach (var r in rows)
        {
            if (!r.Current.HasValue) continue;
            var gross = r.Current.Value;
            var band = bandByGradeLoc.TryGetValue((r.JobGradeId, r.LocationId), out var b1) ? b1
                     : bandByGradeLoc.TryGetValue((r.JobGradeId, null), out var b2) ? b2 : null;
            if (band is null || band.Mid <= 0m || band.Max <= band.Min) continue;

            decimal compa = gross / band.Mid;
            decimal pen = (gross - band.Min) / (band.Max - band.Min);
            string flag = gross < band.Min ? "below" : gross > band.Max ? "above" : "in";
            stats.Add((r.Id, r.EmployeeNumber, r.FullName, r.Department, gross, band.Min, band.Mid, band.Max, compa, pen, flag));
        }

        int empWithSalary = rows.Count(r => r.Current.HasValue);
        int empWithBand = stats.Count;
        if (empWithBand == 0)
        {
            return new CompPolicyOverview(empWithSalary, 0, 0, 0, 0, 0, 0,
                Array.Empty<CompaHistogramBucket>(), Array.Empty<QuartileBucket>(), Array.Empty<OutOfBandEmployee>());
        }

        decimal avgCompa = Math.Round(stats.Sum(s => s.Compa) / empWithBand, 3);
        var sortedCompa = stats.Select(s => s.Compa).OrderBy(x => x).ToList();
        decimal medianCompa = sortedCompa.Count % 2 == 1
            ? sortedCompa[sortedCompa.Count / 2]
            : Math.Round((sortedCompa[sortedCompa.Count / 2 - 1] + sortedCompa[sortedCompa.Count / 2]) / 2m, 3);

        // Compa histogram bucket'ları
        var buckets = new (string Label, decimal Lo, decimal Hi)[]
        {
            ("< 0.80",      decimal.MinValue, 0.80m),
            ("0.80-0.90",   0.80m, 0.90m),
            ("0.90-0.95",   0.90m, 0.95m),
            ("0.95-1.05",   0.95m, 1.05m),
            ("1.05-1.10",   1.05m, 1.10m),
            ("1.10-1.20",   1.10m, 1.20m),
            ("> 1.20",      1.20m, decimal.MaxValue),
        };
        var histogram = buckets.Select(bk => new CompaHistogramBucket(
            bk.Label,
            stats.Count(s => s.Compa >= bk.Lo && s.Compa < bk.Hi),
            bk.Lo, bk.Hi)).ToList();

        // Range penetration quartile (0-25, 25-50, 50-75, 75-100, dışı)
        int q1 = stats.Count(s => s.Pen >= 0m && s.Pen < 0.25m);
        int q2 = stats.Count(s => s.Pen >= 0.25m && s.Pen < 0.50m);
        int q3 = stats.Count(s => s.Pen >= 0.50m && s.Pen < 0.75m);
        int q4 = stats.Count(s => s.Pen >= 0.75m && s.Pen <= 1m);
        int outQ = stats.Count(s => s.Pen < 0m || s.Pen > 1m);
        var quartiles = new List<QuartileBucket>
        {
            new("Q1 (0-25%)", q1),
            new("Q2 (25-50%)", q2),
            new("Q3 (50-75%)", q3),
            new("Q4 (75-100%)", q4),
            new("Bant Dışı", outQ),
        };

        int below = stats.Count(s => s.Flag == "below");
        int inBand = stats.Count(s => s.Flag == "in");
        int above = stats.Count(s => s.Flag == "above");

        var outOfBand = stats
            .Where(s => s.Flag != "in")
            .OrderBy(s => s.Flag).ThenByDescending(s => s.Compa)
            .Select(s => new OutOfBandEmployee(s.Id, s.No, s.Name, s.Dept, s.Gross, s.Min, s.Max, s.Flag))
            .ToList();

        return new CompPolicyOverview(empWithSalary, empWithBand, avgCompa, medianCompa,
            below, inBand, above, histogram, quartiles, outOfBand);
    }
}
