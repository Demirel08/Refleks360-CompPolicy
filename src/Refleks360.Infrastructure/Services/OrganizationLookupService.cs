using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Abstractions;
using Refleks360.Infrastructure.Persistence;

namespace Refleks360.Infrastructure.Services;

public sealed class OrganizationLookupService(CompDbContext db) : IOrganizationLookupService
{
    public async Task<IReadOnlyList<LookupItem>> GetDepartmentsAsync(CancellationToken ct = default) =>
        await db.Departments.AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(d => new LookupItem(d.Id, d.Name))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<LookupItem>> GetPositionsAsync(CancellationToken ct = default) =>
        await db.Positions.AsNoTracking()
            .OrderBy(p => p.Title)
            .Select(p => new LookupItem(p.Id, p.Title + " (" + p.JobGrade.Code + ")"))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<LookupItem>> GetLocationsAsync(CancellationToken ct = default) =>
        await db.Locations.AsNoTracking()
            .OrderBy(l => l.Name)
            .Select(l => new LookupItem(l.Id, l.Name))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<LookupItem>> GetJobGradesAsync(CancellationToken ct = default) =>
        await db.JobGrades.AsNoTracking()
            .OrderBy(g => g.OrderIndex)
            .Select(g => new LookupItem(g.Id, g.Code + " — " + g.Name))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<LookupItem>> GetJobFamiliesAsync(CancellationToken ct = default) =>
        await db.JobFamilies.AsNoTracking()
            .OrderBy(f => f.Name)
            .Select(f => new LookupItem(f.Id, f.Name))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<LookupItem>> GetManagerCandidatesAsync(int? excludeEmployeeId, CancellationToken ct = default) =>
        await db.Employees.AsNoTracking()
            .Where(e => excludeEmployeeId == null || e.Id != excludeEmployeeId)
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .Select(e => new LookupItem(e.Id, e.FirstName + " " + e.LastName + " (" + e.EmployeeNumber + ")"))
            .ToListAsync(ct);
}
