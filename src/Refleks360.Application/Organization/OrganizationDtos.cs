using Refleks360.Domain.Organization;

namespace Refleks360.Application.Organization;

public sealed record DepartmentDto(int Id, string Name, int? ParentDepartmentId, string? ParentName, int? ManagerEmployeeId, string? ManagerName, string? CostCenterCode, bool IsActive, int EmployeeCount);
public sealed record PositionDto(int Id, string Title, int JobGradeId, string JobGradeCode, int JobFamilyId, string JobFamilyName, string? BenchmarkMatchName, bool IsActive, int EmployeeCount);
public sealed record LocationDto(int Id, string Name, string City, string Country, decimal RegionalIndexPercent, bool IsActive, int EmployeeCount);
public sealed record JobGradeDto(int Id, string Code, string Name, CareerBand CareerBand, int OrderIndex, int? EvaluationScoreMin, int? EvaluationScoreMax);
public sealed record JobFamilyDto(int Id, string Name, string? Description);
