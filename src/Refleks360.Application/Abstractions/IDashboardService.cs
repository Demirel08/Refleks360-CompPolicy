using Refleks360.Application.Dashboards;

namespace Refleks360.Application.Abstractions;

public interface IDashboardService
{
    Task<DashboardSummary> GetSummaryAsync(CancellationToken ct = default);
}
