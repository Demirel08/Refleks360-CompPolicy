using Refleks360.Application.TotalRewards;
using Refleks360.Domain.Organization;

namespace Refleks360.Application.Abstractions;

public interface ITotalRewardsService
{
    Task<TotalRewardsSummary?> GetForEmployeeAsync(int employeeId, CancellationToken ct = default);
    Task<int> AddBenefitAsync(int employeeId, BenefitType type, decimal monthlyValue, string? description, DateOnly effectiveDate, DateOnly? endDate, CancellationToken ct = default);
    Task DeleteBenefitAsync(int benefitId, CancellationToken ct = default);

    Task<byte[]> GenerateCompLetterPdfAsync(int employeeId, decimal oldGross, decimal newGross, DateOnly effectiveDate, string reason, string signedByUserName, CancellationToken ct = default);
}
