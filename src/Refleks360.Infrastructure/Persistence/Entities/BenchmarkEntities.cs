using Refleks360.Domain.Benchmark;

namespace Refleks360.Infrastructure.Persistence.Entities;

public sealed class BenchmarkProviderEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public BenchmarkSourceType SourceType { get; set; }
}

public sealed class BenchmarkDataEntity
{
    public int Id { get; set; }
    public int BenchmarkProviderId { get; set; }
    public string BenchmarkPositionName { get; set; } = string.Empty;
    public string? Sector { get; set; }
    public string? Region { get; set; }
    public string? CompanySize { get; set; }
    public decimal P25 { get; set; }
    public decimal P50 { get; set; }
    public decimal P75 { get; set; }
    public string Currency { get; set; } = "TL";
    public DateOnly EffectiveDate { get; set; }
    public string? ImportedBy { get; set; }
    public DateTime ImportedAtUtc { get; set; } = DateTime.UtcNow;

    public BenchmarkProviderEntity Provider { get; set; } = default!;
}

public sealed class PositionBenchmarkMappingEntity
{
    public int Id { get; set; }
    public int PositionId { get; set; }
    public int BenchmarkDataId { get; set; }
    public BenchmarkMatchType MatchType { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public PositionEntity Position { get; set; } = default!;
    public BenchmarkDataEntity BenchmarkData { get; set; } = default!;
}
