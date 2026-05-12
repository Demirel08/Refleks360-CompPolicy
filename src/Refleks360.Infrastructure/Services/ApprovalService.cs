using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Abstractions;
using Refleks360.Application.Approvals;
using Refleks360.Domain.Approvals;
using Refleks360.Infrastructure.Persistence;
using Refleks360.Infrastructure.Persistence.Entities;

namespace Refleks360.Infrastructure.Services;

public sealed class ApprovalService(CompDbContext db) : IApprovalService
{
    public async Task<IReadOnlyList<ApprovalTemplateInput>> GetTemplatesAsync(CancellationToken ct = default)
    {
        var list = await db.ApprovalTemplates.AsNoTracking()
            .OrderBy(t => t.Name).ToListAsync(ct);
        return list.Select(t => new ApprovalTemplateInput
        {
            Id = t.Id, Name = t.Name, TriggerType = t.TriggerType,
            AmountThreshold = t.AmountThreshold, IsActive = t.IsActive,
            StepRoles = JsonSerializer.Deserialize<List<string>>(t.StepsJson) ?? new()
        }).ToList();
    }

    public async Task<int> UpsertTemplateAsync(ApprovalTemplateInput input, CancellationToken ct = default)
    {
        ApprovalTemplateEntity entity;
        if (input.Id is null) { entity = new ApprovalTemplateEntity(); db.ApprovalTemplates.Add(entity); }
        else entity = await db.ApprovalTemplates.FirstAsync(t => t.Id == input.Id, ct);

        entity.Name = input.Name;
        entity.TriggerType = input.TriggerType;
        entity.AmountThreshold = input.AmountThreshold;
        entity.IsActive = input.IsActive;
        entity.StepsJson = JsonSerializer.Serialize(input.StepRoles);
        await db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task DeleteTemplateAsync(int id, CancellationToken ct = default)
    {
        var t = await db.ApprovalTemplates.FirstAsync(x => x.Id == id, ct);
        db.ApprovalTemplates.Remove(t);
        await db.SaveChangesAsync(ct);
    }

    public async Task<int> RequestApprovalAsync(string entityType, string entityId, string title, ApprovalTriggerType triggerType, decimal? amount, string requestedBy, string? notes, CancellationToken ct = default)
    {
        // Uygun şablon: trigger tipi eşleşen + amount threshold geçerli + aktif
        var templates = await db.ApprovalTemplates
            .Where(t => t.IsActive && t.TriggerType == triggerType)
            .OrderByDescending(t => t.AmountThreshold ?? 0m)
            .ToListAsync(ct);

        var template = templates.FirstOrDefault(t => !t.AmountThreshold.HasValue || (amount ?? 0m) >= t.AmountThreshold.Value)
                     ?? templates.FirstOrDefault();

        var steps = template is not null
            ? JsonSerializer.Deserialize<List<string>>(template.StepsJson) ?? new()
            : new List<string> { "HRDirector" }; // default fallback

        var approval = new ApprovalEntity
        {
            TemplateId = template?.Id,
            EntityType = entityType,
            EntityId = entityId,
            Title = title,
            CurrentStep = 0,
            TotalSteps = steps.Count,
            Status = ApprovalStatus.Pending,
            RequestedBy = requestedBy,
            Notes = notes,
        };
        db.Approvals.Add(approval);
        await db.SaveChangesAsync(ct);

        for (int i = 0; i < steps.Count; i++)
        {
            db.ApprovalSteps.Add(new ApprovalStepEntity
            {
                ApprovalId = approval.Id, StepIndex = i, RoleName = steps[i],
            });
        }
        await db.SaveChangesAsync(ct);
        return approval.Id;
    }

    public async Task<IReadOnlyList<ApprovalListItem>> GetInboxAsync(string roleName, CancellationToken ct = default)
    {
        var approvals = await db.Approvals.AsNoTracking()
            .Include(a => a.Steps)
            .Where(a => a.Status == ApprovalStatus.Pending && a.Steps.Any(s => s.StepIndex == a.CurrentStep && s.RoleName == roleName))
            .OrderByDescending(a => a.RequestedAtUtc)
            .ToListAsync(ct);
        return approvals.Select(MapList).ToList();
    }

    public async Task<IReadOnlyList<ApprovalListItem>> GetAllAsync(CancellationToken ct = default)
    {
        var approvals = await db.Approvals.AsNoTracking()
            .Include(a => a.Steps)
            .OrderByDescending(a => a.RequestedAtUtc)
            .ToListAsync(ct);
        return approvals.Select(MapList).ToList();
    }

    public async Task<ApprovalDetail?> GetDetailAsync(int id, CancellationToken ct = default)
    {
        var a = await db.Approvals.AsNoTracking()
            .Include(x => x.Steps)
            .Include(x => x.Actions)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (a is null) return null;
        return new ApprovalDetail(
            a.Id, a.Title, a.EntityType, a.EntityId, a.Status, a.CurrentStep, a.TotalSteps,
            a.RequestedBy, a.RequestedAtUtc, a.Notes,
            a.Steps.OrderBy(s => s.StepIndex).Select(s => new ApprovalStepRow(s.StepIndex, s.RoleName, s.IsCompleted)).ToList(),
            a.Actions.OrderBy(x => x.ActionAtUtc).Select(x => new ApprovalActionRow(x.StepIndex, x.ActorUserName, x.Decision, x.Comment, x.ActionAtUtc)).ToList());
    }

    public async Task<ApprovalStatus> ActAsync(int approvalId, ApprovalDecision decision, string actorUserName, string actorRole, string? comment, CancellationToken ct = default)
    {
        var approval = await db.Approvals.Include(a => a.Steps).FirstAsync(a => a.Id == approvalId, ct);
        if (approval.Status != ApprovalStatus.Pending)
            throw new InvalidOperationException("Bu onay sürecinin durumu artık değiştirilemez.");

        var currentStep = approval.Steps.FirstOrDefault(s => s.StepIndex == approval.CurrentStep);
        if (currentStep is null)
            throw new InvalidOperationException("Aktif step bulunamadı.");

        if (!string.IsNullOrEmpty(currentStep.RoleName) && !string.Equals(currentStep.RoleName, actorRole, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException($"Bu adımı sadece '{currentStep.RoleName}' rolü onaylayabilir.");

        db.ApprovalActions.Add(new ApprovalActionEntity
        {
            ApprovalId = approvalId, StepIndex = approval.CurrentStep,
            ActorUserName = actorUserName, Decision = decision, Comment = comment,
        });

        switch (decision)
        {
            case ApprovalDecision.Approve:
                currentStep.IsCompleted = true;
                if (approval.CurrentStep + 1 >= approval.TotalSteps)
                {
                    approval.Status = ApprovalStatus.Approved;
                }
                else
                {
                    approval.CurrentStep++;
                }
                break;
            case ApprovalDecision.Reject:
                approval.Status = ApprovalStatus.Rejected;
                break;
            case ApprovalDecision.Return:
                if (approval.CurrentStep > 0) approval.CurrentStep--;
                break;
        }

        await db.SaveChangesAsync(ct);
        return approval.Status;
    }

    private static ApprovalListItem MapList(ApprovalEntity a) => new(
        a.Id, a.Title, a.EntityType, a.EntityId, a.CurrentStep, a.TotalSteps, a.Status,
        a.Steps.FirstOrDefault(s => s.StepIndex == a.CurrentStep)?.RoleName ?? "-",
        a.RequestedBy, a.RequestedAtUtc);
}
