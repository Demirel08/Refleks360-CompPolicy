using Refleks360.Domain.Approvals;

namespace Refleks360.Application.Approvals;

public sealed record ApprovalListItem(
    int Id,
    string Title,
    string EntityType,
    string EntityId,
    int CurrentStep,
    int TotalSteps,
    ApprovalStatus Status,
    string CurrentApproverRole,
    string RequestedBy,
    DateTime RequestedAtUtc);

public sealed record ApprovalActionRow(int StepIndex, string ActorUserName, ApprovalDecision Decision, string? Comment, DateTime ActionAtUtc);
public sealed record ApprovalStepRow(int StepIndex, string RoleName, bool IsCompleted);

public sealed record ApprovalDetail(
    int Id,
    string Title,
    string EntityType,
    string EntityId,
    ApprovalStatus Status,
    int CurrentStep,
    int TotalSteps,
    string RequestedBy,
    DateTime RequestedAtUtc,
    string? Notes,
    IReadOnlyList<ApprovalStepRow> Steps,
    IReadOnlyList<ApprovalActionRow> Actions);

public sealed class ApprovalTemplateInput
{
    public int? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ApprovalTriggerType TriggerType { get; set; }
    public decimal? AmountThreshold { get; set; }
    public bool IsActive { get; set; } = true;
    /// <summary>Sıralı rol listesi (her step için).</summary>
    public List<string> StepRoles { get; set; } = new();
}
