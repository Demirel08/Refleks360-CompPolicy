using Refleks360.Domain.Organization;

namespace Refleks360.Infrastructure.Persistence.Entities;

/// <summary>
/// Çalışan. Hassas alanlar (NationalId, BirthDate) Hafta 17/20'de Always Encrypted'a alınır;
/// şu an minimal sürüm.
/// </summary>
public sealed class EmployeeEntity
{
    public int Id { get; set; }

    /// <summary>Sicil no — şirket içinde benzersiz.</summary>
    public string EmployeeNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public Gender Gender { get; set; } = Gender.NotSpecified;
    public EmploymentType EmploymentType { get; set; } = EmploymentType.Permanent;
    public WorkSchedule WorkSchedule { get; set; } = WorkSchedule.FullTime;
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

    public DateOnly HireDate { get; set; }
    public DateOnly? TerminationDate { get; set; }

    public int PositionId { get; set; }
    public int DepartmentId { get; set; }
    public int LocationId { get; set; }

    public int? ManagerId { get; set; }
    public bool IsLocked { get; set; }
    public string? Notes { get; set; }

    /// <summary>Soft delete bayrağı; EF global filter ile her query'den otomatik çıkarılır.</summary>
    public bool IsDeleted { get; set; }

    public PositionEntity Position { get; set; } = default!;
    public DepartmentEntity Department { get; set; } = default!;
    public LocationEntity Location { get; set; } = default!;
    public EmployeeEntity? Manager { get; set; }
    public List<EmployeeEntity> DirectReports { get; set; } = new();
}
