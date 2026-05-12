using Refleks360.Domain.Organization;

namespace Refleks360.Application.Employees;

/// <summary>Bir çalışanın detay sayfası için tam veri DTO'su.</summary>
public sealed record EmployeeDetail(
    int Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    Gender Gender,
    EmploymentType EmploymentType,
    WorkSchedule WorkSchedule,
    EmployeeStatus Status,
    DateOnly HireDate,
    DateOnly? TerminationDate,
    int PositionId,
    string PositionTitle,
    string JobGradeCode,
    int DepartmentId,
    string DepartmentName,
    int LocationId,
    string LocationName,
    int? ManagerId,
    string? ManagerFullName,
    bool IsLocked,
    string? Notes);
