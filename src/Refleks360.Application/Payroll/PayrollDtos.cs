namespace Refleks360.Application.Payroll;

/// <summary>
/// Bir çalışan için tüm ücret rakamlarını tek satırda gösteren view DTO'su.
/// Toplu ücret görünümü ve senaryo karşılaştırma sayfalarında kullanılır.
/// </summary>
public sealed record EmployeePayrollRow(
    int Id,
    string EmployeeNumber,
    string FullName,
    string Department,
    string Position,
    string GradeCode,
    decimal? Gross,
    decimal? Net,
    decimal? EmployerCost,
    decimal? CompaRatio,
    string? BandFlag);

public sealed record PayrollSnapshot(
    int Year,
    int Headcount,
    decimal TotalGross,
    decimal TotalNet,
    decimal TotalEmployerCost,
    IReadOnlyList<EmployeePayrollRow> Rows);
