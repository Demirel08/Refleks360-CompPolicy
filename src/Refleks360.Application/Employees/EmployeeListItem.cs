using Refleks360.Domain.Organization;

namespace Refleks360.Application.Employees;

/// <summary>
/// Çalışan liste görünümü için minimal projeksiyon. Ücret bilgisi
/// burada yok — bant/maaş yetkisi olan kullanıcılar için ayrı bir DTO eklenecek.
/// </summary>
public sealed record EmployeeListItem(
    int Id,
    string EmployeeNumber,
    string FullName,
    string PositionTitle,
    string JobGradeCode,
    string DepartmentName,
    string LocationCity,
    EmployeeStatus Status,
    Gender Gender,
    DateOnly HireDate);
