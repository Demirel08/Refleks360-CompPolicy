using Refleks360.Domain.Organization;

namespace Refleks360.Infrastructure.Persistence.Entities;

/// <summary>
/// Çalışanın ücret geçmişi (history desenli). EndDate=null olan satır halen geçerli olandır;
/// yeni bir ücret eklendiğinde önceki satırın EndDate'i doldurulur.
/// </summary>
public sealed class EmployeeSalaryEntity
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public decimal GrossMonthly { get; set; }
    public decimal? NetMonthlyCached { get; set; }
    public decimal? EmployerCostCached { get; set; }
    public DateOnly EffectiveDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public SalaryChangeReason Reason { get; set; } = SalaryChangeReason.NewHire;
    public decimal? ChangePercent { get; set; }
    public decimal? ChangeAmount { get; set; }
    public int? ScenarioId { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public EmployeeEntity Employee { get; set; } = default!;
}
