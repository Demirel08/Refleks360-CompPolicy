using Refleks360.Application.Organization;

namespace Refleks360.Application.Abstractions;

public interface ISalaryBandService
{
    Task<IReadOnlyList<SalaryBandDto>> GetAllAsync(CancellationToken ct = default);
    Task<int> UpsertAsync(int? id, int jobGradeId, int? locationId, decimal min, decimal mid, decimal max, DateOnly effectiveDate, DateOnly? endDate, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);

    /// <summary>İlk bant + range spread + mid progression → kalan kademelerin bantları otomatik üret.</summary>
    Task<int> AutoGenerateAsync(int firstGradeId, decimal firstMin, decimal firstMid, decimal firstMax, decimal midProgressionPercent, DateOnly effectiveDate, CancellationToken ct = default);
}
