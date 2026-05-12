namespace Refleks360.Application.CompPolicy;

/// <summary>Bir kova (compa-ratio aralığı) içindeki çalışan sayısı.</summary>
public sealed record CompaHistogramBucket(string Label, int Count, decimal LowerInclusive, decimal UpperExclusive);

/// <summary>Quartile dağılımı (Q1-Q4 + dışı).</summary>
public sealed record QuartileBucket(string Label, int Count);

/// <summary>Bant dışı (below/above) çalışan listesi.</summary>
public sealed record OutOfBandEmployee(int Id, string EmployeeNumber, string FullName, string Department, decimal CurrentGross, decimal BandMin, decimal BandMax, string Flag);

/// <summary>Comp policy ana metrik özetleri.</summary>
public sealed record CompPolicyOverview(
    int EmployeesWithSalary,
    int EmployeesWithBand,
    decimal AverageCompaRatio,
    decimal MedianCompaRatio,
    int BelowBandCount,
    int InBandCount,
    int AboveBandCount,
    IReadOnlyList<CompaHistogramBucket> Histogram,
    IReadOnlyList<QuartileBucket> Quartiles,
    IReadOnlyList<OutOfBandEmployee> OutOfBand);
