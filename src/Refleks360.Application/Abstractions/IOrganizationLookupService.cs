namespace Refleks360.Application.Abstractions;

/// <summary>Dropdown'lar ve select listeleri için hafif lookup veri kaynağı.</summary>
public interface IOrganizationLookupService
{
    Task<IReadOnlyList<LookupItem>> GetDepartmentsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<LookupItem>> GetPositionsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<LookupItem>> GetLocationsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<LookupItem>> GetJobGradesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<LookupItem>> GetJobFamiliesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<LookupItem>> GetManagerCandidatesAsync(int? excludeEmployeeId, CancellationToken ct = default);
}

/// <summary>(Id, görünür ad) ikilisi.</summary>
public sealed record LookupItem(int Id, string Name);
