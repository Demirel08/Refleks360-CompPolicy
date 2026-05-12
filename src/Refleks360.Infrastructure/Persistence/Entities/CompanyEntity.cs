namespace Refleks360.Infrastructure.Persistence.Entities;

/// <summary>
/// Şirket (single-tenant kurulumda tek satır; multi-company holding ileride).
/// </summary>
public sealed class CompanyEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string TaxNo { get; set; } = string.Empty;
    public int FiscalYearStartMonth { get; set; } = 1;
    public string Currency { get; set; } = "TL";
    public bool IsActive { get; set; } = true;

    public List<LocationEntity> Locations { get; set; } = new();
    public List<DepartmentEntity> Departments { get; set; } = new();
}
