namespace Refleks360.Infrastructure.Persistence.Entities;

/// <summary>Departman — kendi kendine hiyerarşi (ParentDepartmentId).</summary>
public sealed class DepartmentEntity
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public int? ParentDepartmentId { get; set; }
    public int? ManagerEmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CostCenterCode { get; set; }
    public bool IsActive { get; set; } = true;

    public CompanyEntity Company { get; set; } = default!;
    public DepartmentEntity? ParentDepartment { get; set; }
    public List<DepartmentEntity> ChildDepartments { get; set; } = new();
    public EmployeeEntity? ManagerEmployee { get; set; }
}
