using Refleks360.Domain.Scenarios;

namespace Refleks360.Infrastructure.Persistence.Entities;

public sealed class ScenarioEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly BaseDate { get; set; }
    public ScenarioType Type { get; set; }
    public ScenarioStatus Status { get; set; } = ScenarioStatus.Draft;

    /// <summary>Senaryo tipine özel parametreler (JSON; servis tarafında okunur).</summary>
    public string ParametersJson { get; set; } = "{}";

    public DateOnly? EffectiveDate { get; set; }
    public string CreatedBy { get; set; } = "system";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? AppliedAtUtc { get; set; }
    public string? AppliedBy { get; set; }

    public List<ScenarioEmployeeEntity> Employees { get; set; } = new();
}

public sealed class ScenarioEmployeeEntity
{
    public int Id { get; set; }
    public int ScenarioId { get; set; }
    public int EmployeeId { get; set; }

    public decimal OldGross { get; set; }
    public decimal NewGross { get; set; }
    public decimal OldNetMonthly { get; set; }
    public decimal NewNetMonthly { get; set; }
    public decimal OldEmployerCost { get; set; }
    public decimal NewEmployerCost { get; set; }
    public decimal RaisePercent { get; set; }
    public decimal RaiseAmount { get; set; }
    public bool IsLocked { get; set; }
    public bool IsManuallyOverridden { get; set; }
    public string? Notes { get; set; }

    public ScenarioEntity Scenario { get; set; } = default!;
    public EmployeeEntity Employee { get; set; } = default!;
}
