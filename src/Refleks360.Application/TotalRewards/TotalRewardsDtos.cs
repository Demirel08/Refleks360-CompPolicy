using Refleks360.Domain.Organization;

namespace Refleks360.Application.TotalRewards;

public sealed record BenefitDto(int Id, int EmployeeId, BenefitType BenefitType, decimal MonthlyValue, string? Description, DateOnly EffectiveDate, DateOnly? EndDate);

public sealed record TotalRewardsSummary(
    int EmployeeId,
    string FullName,
    decimal MonthlyGross,
    decimal AnnualGross,
    decimal MonthlyBenefitsTotal,
    decimal AnnualEmployerCost,
    decimal AnnualTotalCompensation,
    IReadOnlyList<BenefitDto> Benefits);
