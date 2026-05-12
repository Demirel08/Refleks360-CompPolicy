namespace Refleks360.Infrastructure.Persistence.Entities;

/// <summary>
/// Bir kademe için (opsiyonel lokasyon kırılımıyla) min/mid/max maaş bantı.
/// EffectiveDate ile sürümlenir; EndDate=null = halen geçerli.
/// </summary>
public sealed class SalaryBandEntity
{
    public int Id { get; set; }
    public int JobGradeId { get; set; }
    public int? LocationId { get; set; }

    public decimal Min { get; set; }
    public decimal Mid { get; set; }
    public decimal Max { get; set; }

    public DateOnly EffectiveDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public JobGradeEntity JobGrade { get; set; } = default!;
    public LocationEntity? Location { get; set; }
}
