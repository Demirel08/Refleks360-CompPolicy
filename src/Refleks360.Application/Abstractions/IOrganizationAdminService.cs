using Refleks360.Application.Organization;
using Refleks360.Domain.Organization;

namespace Refleks360.Application.Abstractions;

/// <summary>
/// Departman / pozisyon / kademe / aile / lokasyon admin CRUD'ları için tek noktadan
/// servis. Düz Blazor formlarının doğrudan çağıracağı şekilde sade DTO'lar.
/// </summary>
public interface IOrganizationAdminService
{
    // Departments
    Task<IReadOnlyList<DepartmentDto>> GetDepartmentsAsync(CancellationToken ct = default);
    Task<int> UpsertDepartmentAsync(int? id, string name, int? parentId, int? managerEmployeeId, string? costCenter, bool isActive, CancellationToken ct = default);
    Task DeleteDepartmentAsync(int id, CancellationToken ct = default);

    // Positions
    Task<IReadOnlyList<PositionDto>> GetPositionsAsync(CancellationToken ct = default);
    Task<int> UpsertPositionAsync(int? id, string title, int jobGradeId, int jobFamilyId, string? benchmark, bool isActive, CancellationToken ct = default);
    Task DeletePositionAsync(int id, CancellationToken ct = default);

    // Locations
    Task<IReadOnlyList<LocationDto>> GetLocationsAsync(CancellationToken ct = default);
    Task<int> UpsertLocationAsync(int? id, string name, string city, string country, decimal regionalIndex, bool isActive, CancellationToken ct = default);
    Task DeleteLocationAsync(int id, CancellationToken ct = default);

    // Job grades
    Task<IReadOnlyList<JobGradeDto>> GetJobGradesAsync(CancellationToken ct = default);
    Task<int> UpsertJobGradeAsync(int? id, string code, string name, CareerBand band, int order, int? scoreMin, int? scoreMax, CancellationToken ct = default);
    Task DeleteJobGradeAsync(int id, CancellationToken ct = default);

    // Job families
    Task<IReadOnlyList<JobFamilyDto>> GetJobFamiliesAsync(CancellationToken ct = default);
    Task<int> UpsertJobFamilyAsync(int? id, string name, string? description, CancellationToken ct = default);
    Task DeleteJobFamilyAsync(int id, CancellationToken ct = default);
}
