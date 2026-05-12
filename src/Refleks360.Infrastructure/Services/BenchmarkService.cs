using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Abstractions;
using Refleks360.Application.Benchmark;
using Refleks360.Domain.Benchmark;
using Refleks360.Domain.Organization;
using Refleks360.Infrastructure.Persistence;
using Refleks360.Infrastructure.Persistence.Entities;
using Syncfusion.XlsIO;

namespace Refleks360.Infrastructure.Services;

public sealed class BenchmarkService(CompDbContext db) : IBenchmarkService
{
    public async Task<IReadOnlyList<BenchmarkProviderDto>> GetProvidersAsync(CancellationToken ct = default)
    {
        var counts = await db.BenchmarkData.GroupBy(d => d.BenchmarkProviderId)
            .Select(g => new { g.Key, Cnt = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Cnt, ct);
        return await db.BenchmarkProviders.AsNoTracking()
            .OrderBy(p => p.Name)
            .Select(p => new BenchmarkProviderDto(p.Id, p.Name, p.SourceType, counts.ContainsKey(p.Id) ? counts[p.Id] : 0))
            .ToListAsync(ct);
    }

    public async Task<int> UpsertProviderAsync(int? id, string name, BenchmarkSourceType sourceType, CancellationToken ct = default)
    {
        BenchmarkProviderEntity p;
        if (id is null) { p = new BenchmarkProviderEntity(); db.BenchmarkProviders.Add(p); }
        else p = await db.BenchmarkProviders.FirstAsync(x => x.Id == id, ct);
        p.Name = name; p.SourceType = sourceType;
        await db.SaveChangesAsync(ct);
        return p.Id;
    }

    public async Task DeleteProviderAsync(int id, CancellationToken ct = default)
    {
        var p = await db.BenchmarkProviders.FirstAsync(x => x.Id == id, ct);
        db.BenchmarkProviders.Remove(p);
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<BenchmarkDataDto>> GetDataAsync(int? providerId, CancellationToken ct = default)
    {
        var q = db.BenchmarkData.AsNoTracking();
        if (providerId is not null) q = q.Where(d => d.BenchmarkProviderId == providerId);
        return await q.OrderBy(d => d.BenchmarkPositionName)
            .Select(d => new BenchmarkDataDto(d.Id, d.BenchmarkProviderId, d.Provider.Name, d.BenchmarkPositionName, d.Sector, d.Region, d.CompanySize, d.P25, d.P50, d.P75, d.Currency, d.EffectiveDate))
            .ToListAsync(ct);
    }

    public async Task<int> UpsertDataAsync(int? id, int providerId, string benchmarkPositionName, string? sector, string? region, string? companySize, decimal p25, decimal p50, decimal p75, string currency, DateOnly effectiveDate, string importedBy, CancellationToken ct = default)
    {
        BenchmarkDataEntity e;
        if (id is null) { e = new BenchmarkDataEntity(); db.BenchmarkData.Add(e); }
        else e = await db.BenchmarkData.FirstAsync(x => x.Id == id, ct);
        e.BenchmarkProviderId = providerId;
        e.BenchmarkPositionName = benchmarkPositionName;
        e.Sector = sector; e.Region = region; e.CompanySize = companySize;
        e.P25 = p25; e.P50 = p50; e.P75 = p75;
        e.Currency = currency;
        e.EffectiveDate = effectiveDate;
        e.ImportedBy = importedBy;
        await db.SaveChangesAsync(ct);
        return e.Id;
    }

    public async Task DeleteDataAsync(int id, CancellationToken ct = default)
    {
        var e = await db.BenchmarkData.FirstAsync(x => x.Id == id, ct);
        db.BenchmarkData.Remove(e);
        await db.SaveChangesAsync(ct);
    }

    public async Task<int> ImportXlsxAsync(int providerId, byte[] xlsxBytes, string importedBy, CancellationToken ct = default)
    {
        using var engine = new ExcelEngine();
        engine.Excel.DefaultVersion = ExcelVersion.Xlsx;
        using var ms = new MemoryStream(xlsxBytes);
        var book = engine.Excel.Workbooks.Open(ms);
        var sheet = book.Worksheets[0];
        int last = sheet.UsedRange.LastRow;
        int n = 0;
        for (int r = 2; r <= last; r++)
        {
            var name = sheet[r, 1].Value?.Trim();
            if (string.IsNullOrWhiteSpace(name)) continue;
            decimal p25 = (decimal)(sheet[r, 5].Number);
            decimal p50 = (decimal)(sheet[r, 6].Number);
            decimal p75 = (decimal)(sheet[r, 7].Number);
            db.BenchmarkData.Add(new BenchmarkDataEntity
            {
                BenchmarkProviderId = providerId,
                BenchmarkPositionName = name,
                Sector = sheet[r, 2].Value?.Trim(),
                Region = sheet[r, 3].Value?.Trim(),
                CompanySize = sheet[r, 4].Value?.Trim(),
                P25 = p25, P50 = p50, P75 = p75,
                Currency = sheet[r, 8].Value?.Trim() is { Length: > 0 } cur ? cur : "TL",
                EffectiveDate = sheet[r, 9].HasDateTime ? DateOnly.FromDateTime(sheet[r, 9].DateTime) : DateOnly.FromDateTime(DateTime.Today),
                ImportedBy = importedBy,
            });
            n++;
        }
        await db.SaveChangesAsync(ct);
        return n;
    }

    public async Task<int> MapPositionAsync(int positionId, int benchmarkDataId, BenchmarkMatchType matchType, CancellationToken ct = default)
    {
        // Eski mappingi sil
        var olds = await db.PositionBenchmarkMappings.Where(m => m.PositionId == positionId).ToListAsync(ct);
        db.PositionBenchmarkMappings.RemoveRange(olds);
        var m = new PositionBenchmarkMappingEntity { PositionId = positionId, BenchmarkDataId = benchmarkDataId, MatchType = matchType };
        db.PositionBenchmarkMappings.Add(m);
        await db.SaveChangesAsync(ct);
        return m.Id;
    }

    public async Task RemoveMappingAsync(int mappingId, CancellationToken ct = default)
    {
        var m = await db.PositionBenchmarkMappings.FirstAsync(x => x.Id == mappingId, ct);
        db.PositionBenchmarkMappings.Remove(m);
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<MarketIndexRow>> ComputeMarketIndexAsync(CancellationToken ct = default)
    {
        var positions = await db.Positions.AsNoTracking().ToListAsync(ct);
        var employees = await db.Employees.AsNoTracking()
            .Where(e => e.Status == EmployeeStatus.Active)
            .Select(e => new { e.PositionId, Gross = db.EmployeeSalaries.Where(s => s.EmployeeId == e.Id && s.EndDate == null).Select(s => (decimal?)s.GrossMonthly).FirstOrDefault() })
            .ToListAsync(ct);
        var mappings = await db.PositionBenchmarkMappings.AsNoTracking()
            .Include(m => m.BenchmarkData)
            .ToListAsync(ct);

        var result = new List<MarketIndexRow>();
        foreach (var p in positions)
        {
            var emps = employees.Where(e => e.PositionId == p.Id && e.Gross.HasValue).Select(e => e.Gross!.Value).OrderBy(x => x).ToList();
            decimal median = 0m;
            if (emps.Count > 0)
            {
                median = emps.Count % 2 == 1 ? emps[emps.Count / 2] : (emps[emps.Count / 2 - 1] + emps[emps.Count / 2]) / 2m;
            }
            var map = mappings.FirstOrDefault(m => m.PositionId == p.Id);
            decimal? p50 = map?.BenchmarkData.P50;
            decimal? idx = (p50 is > 0 && emps.Count > 0) ? Math.Round(median / p50.Value, 3) : (decimal?)null;
            result.Add(new MarketIndexRow(p.Id, p.Title, emps.Count, Math.Round(median, 0), p50, idx, map?.BenchmarkData.BenchmarkPositionName));
        }
        return result.OrderBy(r => r.PositionTitle).ToList();
    }
}
