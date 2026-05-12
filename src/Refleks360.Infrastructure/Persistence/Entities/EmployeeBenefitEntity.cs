using Refleks360.Domain.Organization;

namespace Refleks360.Infrastructure.Persistence.Entities;

public sealed class EmployeeBenefitEntity
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public BenefitType BenefitType { get; set; }
    public decimal MonthlyValue { get; set; }
    public string? Description { get; set; }
    public DateOnly EffectiveDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public EmployeeEntity Employee { get; set; } = default!;
}

public sealed class CompensationLetterEntity
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime IssuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateOnly EffectiveDate { get; set; }
    public decimal OldGross { get; set; }
    public decimal NewGross { get; set; }
    public decimal RaisePercent { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public string SignedByUserName { get; set; } = string.Empty;
    public bool SentToEmployee { get; set; }
    public byte[]? PdfBlob { get; set; }

    public EmployeeEntity Employee { get; set; } = default!;
}
