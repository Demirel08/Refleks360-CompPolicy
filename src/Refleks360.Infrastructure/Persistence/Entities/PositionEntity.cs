namespace Refleks360.Infrastructure.Persistence.Entities;

/// <summary>Pozisyon (örn. "Senior Yazılım Mühendisi"). Kademe ve iş ailesine bağlı.</summary>
public sealed class PositionEntity
{
    public int Id { get; set; }
    public int JobGradeId { get; set; }
    public int JobFamilyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? BenchmarkMatchName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public JobGradeEntity JobGrade { get; set; } = default!;
    public JobFamilyEntity JobFamily { get; set; } = default!;
}
