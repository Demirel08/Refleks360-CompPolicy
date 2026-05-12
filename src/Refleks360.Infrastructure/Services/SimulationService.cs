using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Abstractions;
using Refleks360.Application.Simulation;
using Refleks360.Domain.Calculations;
using Refleks360.Infrastructure.Persistence;

namespace Refleks360.Infrastructure.Services;

public sealed class SimulationService(CompDbContext db, ITaxParameterService taxParams) : ISimulationService
{
    // Kıdem tazminatı tavanı 2026 (placeholder — Hafta 19'da ayar ekranı ekleyecek):
    private const decimal SeveranceCeilingMonthly = 50_000m;

    public async Task<SimulationResult> SimulateAsync(
        IReadOnlyList<NewHireInput> hires,
        IReadOnlyList<DepartureInput> departures,
        CancellationToken ct = default)
    {
        var year = DateTime.UtcNow.Year;
        var tax = await taxParams.GetForYearAsync(year, ct);

        decimal AvgAnnualEmployerCost(decimal gross)
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

        var lines = new List<SimulationLine>(hires.Count + departures.Count);
        decimal totalMonthlyDelta = 0m;
        decimal totalSeverance = 0m;

        // Yeni işe alımlar: + maliyet
        foreach (var h in hires)
        {
            var monthly = AvgAnnualEmployerCost(h.MonthlyGross);
            lines.Add(new SimulationLine(h.Label, "hire", monthly, monthly * 12m, 0m));
            totalMonthlyDelta += monthly;
        }

        // Ayrılışlar: - maliyet + kıdem tazminatı
        var empIds = departures.Select(d => d.EmployeeId).ToList();
        if (empIds.Count > 0)
        {
            var data = await db.Employees.AsNoTracking()
                .Where(e => empIds.Contains(e.Id))
                .Select(e => new
                {
                    e.Id,
                    FullName = e.FirstName + " " + e.LastName,
                    e.HireDate,
                    Gross = db.EmployeeSalaries
                        .Where(s => s.EmployeeId == e.Id && s.EndDate == null)
                        .Select(s => (decimal?)s.GrossMonthly).FirstOrDefault()
                })
                .ToListAsync(ct);

            foreach (var d in departures)
            {
                var emp = data.FirstOrDefault(x => x.Id == d.EmployeeId);
                if (emp is null) continue;
                decimal gross = emp.Gross ?? 0m;
                if (gross <= 0m) continue;

                decimal monthly = AvgAnnualEmployerCost(gross);
                lines.Add(new SimulationLine($"{emp.FullName} ayrılıyor", "leave", -monthly, -monthly * 12m,
                    CalculateSeverance(emp.HireDate, d.TerminationDate, gross)));
                totalMonthlyDelta -= monthly;
                totalSeverance += CalculateSeverance(emp.HireDate, d.TerminationDate, gross);
            }
        }

        return new SimulationResult(totalMonthlyDelta, totalMonthlyDelta * 12m, totalSeverance, lines);
    }

    private static decimal CalculateSeverance(DateOnly hire, DateOnly termination, decimal monthlyGross)
    {
        if (termination <= hire) return 0m;
        decimal years = (decimal)(termination.ToDateTime(TimeOnly.MinValue) - hire.ToDateTime(TimeOnly.MinValue)).TotalDays / 365.25m;
        decimal cap = Math.Min(monthlyGross, SeveranceCeilingMonthly);
        return Math.Round(cap * years, 2);
    }
}
