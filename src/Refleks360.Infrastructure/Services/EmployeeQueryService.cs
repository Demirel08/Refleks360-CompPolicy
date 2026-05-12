using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Abstractions;
using Refleks360.Application.Employees;
using Refleks360.Infrastructure.Persistence;

namespace Refleks360.Infrastructure.Services;

public sealed class EmployeeQueryService(CompDbContext db) : IEmployeeQueryService
{
    public async Task<IReadOnlyList<EmployeeListItem>> GetAllAsync(CancellationToken ct = default)
    {
        // 1) Çalışanlar + pozisyon/grade/dept/lokasyon
        var people = await db.Employees
            .AsNoTracking()
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .Select(e => new
            {
                e.Id,
                e.EmployeeNumber,
                FullName = e.FirstName + " " + e.LastName,
                PositionTitle = e.Position.Title,
                JobGradeId = e.Position.JobGradeId,
                JobGradeCode = e.Position.JobGrade.Code,
                DepartmentName = e.Department.Name,
                LocationId = e.LocationId,
                LocationCity = e.Location.City,
                e.Status,
                e.Gender,
                e.HireDate,
            })
            .ToListAsync(ct);

        if (people.Count == 0) return Array.Empty<EmployeeListItem>();

        // 2) Mevcut ücretler (EndDate=null olan satırlar)
        var currentSalaries = await db.EmployeeSalaries
            .AsNoTracking()
            .Where(s => s.EndDate == null)
            .ToDictionaryAsync(s => s.EmployeeId, s => s.GrossMonthly, ct);

        // 3) Geçerli bantlar (lokasyon yoksa null'a düşer)
        var bands = await db.SalaryBands.AsNoTracking()
            .Where(b => b.EndDate == null)
            .ToListAsync(ct);

        var bandByGradeLocation = bands.ToDictionary(b => (b.JobGradeId, b.LocationId), b => b);

        return people.Select(p =>
        {
            decimal? gross = currentSalaries.TryGetValue(p.Id, out var g) ? g : null;
            decimal? compa = null, penetration = null;
            string? flag = null;

            if (gross.HasValue)
            {
                // Önce lokasyon bandı, yoksa genel band
                var band = bandByGradeLocation.TryGetValue((p.JobGradeId, p.LocationId), out var b1)
                    ? b1
                    : bandByGradeLocation.TryGetValue((p.JobGradeId, (int?)null), out var b2) ? b2 : null;

                if (band is not null && band.Mid > 0m && band.Max > band.Min)
                {
                    compa = Math.Round(gross.Value / band.Mid, 3);
                    penetration = Math.Round((gross.Value - band.Min) / (band.Max - band.Min), 3);
                    flag = gross.Value < band.Min ? "below" : gross.Value > band.Max ? "above" : "in";
                }
            }

            return new EmployeeListItem(
                p.Id, p.EmployeeNumber, p.FullName, p.PositionTitle, p.JobGradeCode,
                p.DepartmentName, p.LocationCity, p.Status, p.Gender, p.HireDate,
                gross, compa, penetration, flag);
        }).ToList();
    }
}
