using Refleks360.Domain.Benchmark;

namespace Refleks360.Application.Benchmark;

public sealed record BenchmarkProviderDto(int Id, string Name, BenchmarkSourceType SourceType, int DataPointCount);
public sealed record BenchmarkDataDto(int Id, int ProviderId, string ProviderName, string BenchmarkPositionName, string? Sector, string? Region, string? CompanySize, decimal P25, decimal P50, decimal P75, string Currency, DateOnly EffectiveDate);

public sealed record MarketIndexRow(
    int PositionId,
    string PositionTitle,
    int Headcount,
    decimal CompanyMedian,
    decimal? MarketP50,
    decimal? MarketIndex,
    string? BenchmarkPosition);
