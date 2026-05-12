using Refleks360.Domain.Approvals;

namespace Refleks360.Infrastructure.Persistence.Entities;

public sealed class ApprovalTemplateEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ApprovalTriggerType TriggerType { get; set; }
    /// <summary>JSON: step listesi. Her step için role veya userId, threshold (opsiyonel).</summary>
    public string StepsJson { get; set; } = "[]";
    public decimal? AmountThreshold { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public sealed class ApprovalEntity
{
    public int Id { get; set; }
    public int? TemplateId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int CurrentStep { get; set; }
    public int TotalSteps { get; set; }
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    public ApprovalTemplateEntity? Template { get; set; }
    public List<ApprovalActionEntity> Actions { get; set; } = new();
    public List<ApprovalStepEntity> Steps { get; set; } = new();
}

public sealed class ApprovalStepEntity
{
    public int Id { get; set; }
    public int ApprovalId { get; set; }
    public int StepIndex { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? AssignedUserId { get; set; }
    public bool IsCompleted { get; set; }
    public ApprovalEntity Approval { get; set; } = default!;
}

public sealed class ApprovalActionEntity
{
    public int Id { get; set; }
    public int ApprovalId { get; set; }
    public int StepIndex { get; set; }
    public string ActorUserName { get; set; } = string.Empty;
    public ApprovalDecision Decision { get; set; }
    public string? Comment { get; set; }
    public DateTime ActionAtUtc { get; set; } = DateTime.UtcNow;

    public ApprovalEntity Approval { get; set; } = default!;
}
