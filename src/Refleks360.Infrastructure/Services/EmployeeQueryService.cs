using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Refleks360.Application.Abstractions;
using Refleks360.Application.Employees;
using Refleks360.Infrastructure.Persistence;

namespace Refleks360.Infrastructure.Services;

public sealed class EmployeeQueryService(IDbContextFactory<CompDbContext> dbFactory, IMemoryCache cache) : IEmployeeQueryService
{
    private const string CacheKey = "employee-list-with-compa";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(30);

    public async Task<IReadOnlyList<EmployeeListItem>> GetAllAsync(CancellationToken ct = default)
    {
        if (cache.TryGetValue<IReadOnlyList<EmployeeListItem>>(CacheKey, out var cached) && cached is not null)
        {
            return cached;
        }

        await using var db = await dbFactory.CreateDbContextAsync(ct);

        // 1) Çalışanlar + ilişkiler — AsSplitQuery: çok büyük join yerine 2-3 küçük sorgu
        var people = await db.Employees
            .AsNoTracking()
            .AsSplitQuery()
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

        if (people.Count == 0)
        {
            var empty = Array.Empty<EmployeeListItem>();
            cache.Set(CacheKey, (IReadOnlyList<EmployeeListItem>)empty, CacheTtl);
            return empty;
        }

        // 2) Mevcut ücretler
        var currentSalaries = await db.EmployeeSalaries
            .AsNoTracking()
            .Where(s => s.EndDate == null)
            .Select(s => new { s.EmployeeId, s.GrossMonthly })
            .ToDictionaryAsync(s => s.EmployeeId, s => s.GrossMonthly, ct);

        // 3) Geçerli bantlar
        var bands = await db.SalaryBands.AsNoTracking()
            .Where(b => b.EndDate == null)
            .Select(b => new { b.JobGradeId, b.LocationId, b.Min, b.Mid, b.Max })
            .ToListAsync(ct);

        var bandByGradeLocation = bands.ToDictionary(b => (b.JobGradeId, b.LocationId), b => b);

        var result = people.Select(p =>
        {
            decimal? gross = currentSalaries.TryGetValue(p.Id, out var g) ? g : null;
            decimal? compa = null, penetration = null;
            string? flag = null;

            if (gross.HasValue)
            {
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

        cache.Set(CacheKey, (IReadOnlyList<EmployeeListItem>)result, CacheTtl);
        return result;
    }
}
