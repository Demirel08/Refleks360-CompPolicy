namespace Refleks360.Application.Audit;

public sealed record AuditEntryDto(
    long Id,
    DateTime TimestampUtc,
    string UserName,
    string Action,
    string EntityType,
    string? EntityKey,
    string? OldValuesJson,
    string? NewValuesJson);

public sealed record AuditPage(IReadOnlyList<AuditEntryDto> Entries, int Total);

public sealed class AuditFilter
{
    public string? UserName { get; set; }
    public string? EntityType { get; set; }
    public string? EntityKey { get; set; }
    public DateTime? FromUtc { get; set; }
    public DateTime? ToUtc { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; } = 200;
}
