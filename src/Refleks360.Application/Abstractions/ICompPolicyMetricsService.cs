using Refleks360.Application.CompPolicy;

namespace Refleks360.Application.Abstractions;

public interface ICompPolicyMetricsService
{
    Task<CompPolicyOverview> ComputeOverviewAsync(CancellationToken ct = default);
}
