using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Refleks360.Infrastructure.Persistence.Entities;

namespace Refleks360.Infrastructure.Persistence.Auditing;

/// <summary>
/// SaveChanges öncesinde değişen entity'leri yakalayıp <see cref="AuditLogEntity"/>
/// satırları üretir. Identity ve audit tablolarının kendisi audit edilmez (sonsuz döngü).
/// </summary>
public sealed class AuditSaveChangesInterceptor(IAuditUserContext userContext) : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
    };

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            EmitAuditLogs(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            EmitAuditLogs(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    private void EmitAuditLogs(DbContext context)
    {
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Where(e => !IsAuditExempt(e))
            .ToList();

        if (entries.Count == 0) return;

        var userName = userContext.GetCurrentUserName();
        var timestamp = DateTime.UtcNow;
        var logs = new List<AuditLogEntity>(entries.Count);

        foreach (var entry in entries)
        {
            logs.Add(new AuditLogEntity
            {
                TimestampUtc = timestamp,
                UserName = userName,
                Action = entry.State.ToString(),
                EntityType = entry.Entity.GetType().Name,
                EntityKey = ExtractKey(entry),
                OldValuesJson = entry.State == EntityState.Added ? null : Serialize(entry.OriginalValues),
                NewValuesJson = entry.State == EntityState.Deleted ? null : Serialize(entry.CurrentValues),
            });
        }

        context.Set<AuditLogEntity>().AddRange(logs);
    }

    private static bool IsAuditExempt(EntityEntry entry)
    {
        var type = entry.Entity.GetType();

        // Audit log'un kendisi audit edilmez.
        if (type == typeof(AuditLogEntity)) return true;

        // Identity tabloları audit edilmez (hash, token vs. hassas + gürültü).
        var ns = type.Namespace;
        if (ns is not null && ns.StartsWith("Microsoft.AspNetCore.Identity", StringComparison.Ordinal))
            return true;

        return false;
    }

    private static string? ExtractKey(EntityEntry entry)
    {
        var keyValues = entry.Metadata.FindPrimaryKey()?.Properties
            .Select(p => entry.Property(p.Name).CurrentValue?.ToString())
            .Where(v => v is not null)
            .ToList();

        return keyValues is null or { Count: 0 } ? null : string.Join("|", keyValues);
    }

    private static string Serialize(PropertyValues values)
    {
        var dict = new Dictionary<string, object?>(values.Properties.Count);
        foreach (var prop in values.Properties)
        {
            dict[prop.Name] = values[prop.Name];
        }
        return JsonSerializer.Serialize(dict, JsonOptions);
    }
}
