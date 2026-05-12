using Refleks360.Domain.Scenarios;

namespace Refleks360.Application.Scenarios;

public sealed record ScenarioListItem(
    int Id,
    string Name,
    ScenarioType Type,
    ScenarioStatus Status,
    DateOnly BaseDate,
    DateOnly? EffectiveDate,
    int EmployeeCount,
    decimal TotalOldGross,
    decimal TotalNewGross,
    decimal TotalOldEmployerCost,
    decimal TotalNewEmployerCost,
    string CreatedBy,
    DateTime CreatedAtUtc);

public sealed record ScenarioEmployeeRow(
    int Id,
    int EmployeeId,
    string EmployeeNumber,
    string FullName,
    string DepartmentName,
    string PositionTitle,
    decimal OldGross,
    decimal NewGross,
    decimal OldNet,
    decimal NewNet,
    decimal OldEmployerCost,
    decimal NewEmployerCost,
    decimal RaisePercent,
    decimal RaiseAmount,
    bool IsLocked);

public sealed record ScenarioDetail(
    int Id,
    string Name,
    string? Description,
    ScenarioType Type,
    ScenarioStatus Status,
    DateOnly BaseDate,
    DateOnly? EffectiveDate,
    string ParametersJson,
    string CreatedBy,
    DateTime CreatedAtUtc,
    DateTime? AppliedAtUtc,
    IReadOnlyList<ScenarioEmployeeRow> Employees);

/// <summary>Senaryo oluşturma parametreleri (tip-spesifik alanlarla).</summary>
public sealed class CreateScenarioInput
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ScenarioType Type { get; set; }
    public DateOnly BaseDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly? EffectiveDate { get; set; }

    // GeneralRaise
    public decimal? GeneralRaisePercent { get; set; }
    public bool MinWageFloor { get; set; }
    public decimal? MinWageGross { get; set; }

    // DepartmentRaise: DeptId -> percent
    public Dictionary<int, decimal>? DepartmentRaiseRates { get; set; }

    // PositionRaise: PositionId -> percent
    public Dictionary<int, decimal>? PositionRaiseRates { get; set; }

    // TargetedBudget
    public decimal? TargetTotalAnnualCost { get; set; }
    public decimal? MinGuaranteedPercent { get; set; }   // Mod B (min garantili)
    /// <summary>Senaryoya dahil edilen çalışan Id'leri (boşsa hepsi).</summary>
    public IReadOnlyList<int>? IncludedEmployeeIds { get; set; }
    /// <summary>Kilitli (zammı manuel atanmış) çalışan Id'leri.</summary>
    public IReadOnlyList<int>? LockedEmployeeIds { get; set; }
    public Dictionary<int, decimal>? LockedEmployeePercents { get; set; }
}
