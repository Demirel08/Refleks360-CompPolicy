namespace Refleks360.Application.Organization;

public sealed record SalaryBandDto(
    int Id,
    int JobGradeId,
    string JobGradeCode,
    int? LocationId,
    string? LocationName,
    decimal Min,
    decimal Mid,
    decimal Max,
    DateOnly EffectiveDate,
    DateOnly? EndDate)
{
    public decimal RangeSpreadPercent => Min > 0 ? Math.Round((Max - Min) / Min * 100m, 1) : 0;
}
