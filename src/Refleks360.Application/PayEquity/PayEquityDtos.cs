namespace Refleks360.Application.PayEquity;

public sealed record PayEquityCohort(
    string Label,
    int MaleCount, decimal MaleAvg,
    int FemaleCount, decimal FemaleAvg,
    decimal RawGapPercent);

public sealed record PayEquityResult(
    decimal OverallMaleAvg, decimal OverallFemaleAvg, decimal OverallRawGap,
    decimal AdjustedGapPercent,
    IReadOnlyList<PayEquityCohort> ByGrade,
    IReadOnlyList<PayEquityCohort> ByDepartment);

public sealed record MeritCell(string PerformanceBand, string Quartile, decimal RaisePercent);

public sealed class MeritMatrixInput
{
    public string Name { get; set; } = string.Empty;
    public List<MeritCell> Cells { get; set; } = new();
}
