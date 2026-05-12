using Refleks360.Application.PayEquity;

namespace Refleks360.Application.Abstractions;

public interface IPayEquityService
{
    Task<PayEquityResult> ComputeAsync(CancellationToken ct = default);
}
