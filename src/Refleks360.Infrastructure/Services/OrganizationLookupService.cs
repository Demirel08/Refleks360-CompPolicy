using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Refleks360.Application.Abstractions;
using Refleks360.Infrastructure.Persistence;

namespace Refleks360.Infrastructure.Services;

public sealed class OrganizationLookupService(IDbContextFactory<CompDbContext> dbFactory, IMemoryCache cache) : IOrganizationLookupService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    public Task<IReadOnlyList<LookupItem>> GetDepartmentsAsync(CancellationToken ct = default) =>
        GetCachedAsync("lookup:departments", ct, async db =>
            await db.Departments.AsNoTracking()
                .OrderBy(d => d.Name)
                .Select(d => new LookupItem(d.Id, d.Name))
                .ToListAsync(ct));

    public Task<IReadOnlyList<LookupItem>> GetPositionsAsync(CancellationToken ct = default) =>
        GetCachedAsync("lookup:positions", ct, async db =>
            await db.Positions.AsNoTracking()
                .OrderBy(p => p.Title)
                .Select(p => new LookupItem(p.Id, p.Title + " (" + p.JobGrade.Code + ")"))
                .ToListAsync(ct));

    public Task<IReadOnlyList<LookupItem>> GetLocationsAsync(CancellationToken ct = default) =>
        GetCachedAsync("lookup:locations", ct, async db =>
            await db.Locations.AsNoTracking()
                .OrderBy(l => l.Name)
                .Select(l => new LookupItem(l.Id, l.Name))
                .ToListAsync(ct));

    public Task<IReadOnlyList<LookupItem>> GetJobGradesAsync(CancellationToken ct = default) =>
        GetCachedAsync("lookup:grades", ct, async db =>
            await db.JobGrades.AsNoTracking()
                .OrderBy(g => g.OrderIndex)
                .Select(g => new LookupItem(g.Id, g.Code + " — " + g.Name))
                .ToListAsync(ct));

    public Task<IReadOnlyList<LookupItem>> GetJobFamiliesAsync(CancellationToken ct = default) =>
        GetCachedAsync("lookup:families", ct, async db =>
            await db.JobFamilies.AsNoTracking()
                .OrderBy(f => f.Name)
                .Select(f => new LookupItem(f.Id, f.Name))
                .ToListAsync(ct));

    public async Task<IReadOnlyList<LookupItem>> GetManagerCandidatesAsync(int? excludeEmployeeId, CancellationToken ct = default)
    {
        // Manager listesi sık değişebilir, daha kısa TTL
        var key = $"lookup:managers:{excludeEmployeeId?.ToString() ?? "all"}";
        if (cache.TryGetValue<IReadOnlyList<LookupItem>>(key, out var cached) && cached is not null)
            return cached;

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var list = await db.Employees.AsNoTracking()
            .Where(e => excludeEmployeeId == null || e.Id != excludeEmployeeId)
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .Select(e => new LookupItem(e.Id, e.FirstName + " " + e.LastName + " (" + e.EmployeeNumber + ")"))
            .ToListAsync(ct);

        cache.Set(key, (IReadOnlyList<LookupItem>)list, TimeSpan.FromMinutes(1));
        return list;
    }

    private async Task<IReadOnlyList<LookupItem>> GetCachedAsync(
        string key, CancellationToken ct, Func<CompDbContext, Task<List<LookupItem>>> fetch)
    {
        if (cache.TryGetValue<IReadOnlyList<LookupItem>>(key, out var cached) && cached is not null)
            return cached;
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var list = await fetch(db);
        cache.Set(key, (IReadOnlyList<LookupItem>)list, CacheTtl);
        return list;
    }
}
