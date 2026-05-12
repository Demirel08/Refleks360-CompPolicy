using Refleks360.Application.Benchmark;
using Refleks360.Domain.Benchmark;

namespace Refleks360.Application.Abstractions;

public interface IBenchmarkService
{
    Task<IReadOnlyList<BenchmarkProviderDto>> GetProvidersAsync(CancellationToken ct = default);
    Task<int> UpsertProviderAsync(int? id, string name, BenchmarkSourceType sourceType, CancellationToken ct = default);
    Task DeleteProviderAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<BenchmarkDataDto>> GetDataAsync(int? providerId, CancellationToken ct = default);
    Task<int> UpsertDataAsync(int? id, int providerId, string benchmarkPositionName, string? sector, string? region, string? companySize, decimal p25, decimal p50, decimal p75, string currency, DateOnly effectiveDate, string importedBy, CancellationToken ct = default);
    Task DeleteDataAsync(int id, CancellationToken ct = default);

    Task<int> ImportXlsxAsync(int providerId, byte[] xlsxBytes, string importedBy, CancellationToken ct = default);

    Task<int> MapPositionAsync(int positionId, int benchmarkDataId, BenchmarkMatchType matchType, CancellationToken ct = default);
    Task RemoveMappingAsync(int mappingId, CancellationToken ct = default);

    Task<IReadOnlyList<MarketIndexRow>> ComputeMarketIndexAsync(CancellationToken ct = default);
}
