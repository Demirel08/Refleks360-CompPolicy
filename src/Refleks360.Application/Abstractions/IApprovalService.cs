using Refleks360.Application.Approvals;
using Refleks360.Domain.Approvals;

namespace Refleks360.Application.Abstractions;

public interface IApprovalService
{
    // Templates
    Task<IReadOnlyList<ApprovalTemplateInput>> GetTemplatesAsync(CancellationToken ct = default);
    Task<int> UpsertTemplateAsync(ApprovalTemplateInput input, CancellationToken ct = default);
    Task DeleteTemplateAsync(int id, CancellationToken ct = default);

    // Approvals
    Task<int> RequestApprovalAsync(string entityType, string entityId, string title, ApprovalTriggerType triggerType, decimal? amount, string requestedBy, string? notes, CancellationToken ct = default);
    Task<IReadOnlyList<ApprovalListItem>> GetInboxAsync(string roleName, CancellationToken ct = default);
    Task<IReadOnlyList<ApprovalListItem>> GetAllAsync(CancellationToken ct = default);
    Task<ApprovalDetail?> GetDetailAsync(int id, CancellationToken ct = default);
    Task<ApprovalStatus> ActAsync(int approvalId, ApprovalDecision decision, string actorUserName, string actorRole, string? comment, CancellationToken ct = default);
}
