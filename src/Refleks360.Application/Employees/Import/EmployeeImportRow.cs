using Refleks360.Domain.Organization;

namespace Refleks360.Application.Employees.Import;

/// <summary>Excel'in bir satırından okunan ham veri. Lookup'lar isimle (Title, Name)
/// gelir; servis bunları Id'ye dönüştürür.</summary>
public sealed class EmployeeImportRow
{
    public int RowNumber { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string GenderText { get; set; } = string.Empty;
    public string EmploymentTypeText { get; set; } = string.Empty;
    public string WorkScheduleText { get; set; } = string.Empty;
    public DateOnly? HireDate { get; set; }
    public string PositionTitle { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

/// <summary>Tek satırın özet sonucu (önizleme için).</summary>
public sealed record EmployeeImportRowResult(
    int RowNumber,
    string EmployeeNumber,
    string FullName,
    bool IsNew,
    bool IsValid,
    IReadOnlyList<string> Errors);

/// <summary>Tüm dosyanın özeti.</summary>
public sealed record EmployeeImportPreview(
    int TotalRows,
    int Valid,
    int Invalid,
    int Inserts,
    int Updates,
    IReadOnlyList<EmployeeImportRowResult> Rows);
