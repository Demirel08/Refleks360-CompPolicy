namespace Refleks360.Infrastructure.Persistence.Entities;

/// <summary>
/// CRUD denetim kaydı. EF Core <c>SaveChangesInterceptor</c>'ı her
/// kaydı yazmadan önce eski/yeni değerleri JSON olarak buraya yazar.
/// </summary>
public sealed class AuditLogEntity
{
    public long Id { get; set; }

    /// <summary>Olayın gerçekleştiği UTC zaman.</summary>
    public DateTime TimestampUtc { get; set; }

    /// <summary>İşlemi yapan kullanıcı adı (yoksa "system").</summary>
    public string UserName { get; set; } = "system";

    /// <summary>İşlem tipi: Added / Modified / Deleted.</summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>Etkilenen entity tipi (basit ad).</summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>Etkilenen entity'nin birincil anahtarı (string'e çevrilmiş).</summary>
    public string? EntityKey { get; set; }

    /// <summary>Değişiklik öncesi değerler (JSON; INSERT için null).</summary>
    public string? OldValuesJson { get; set; }

    /// <summary>Değişiklik sonrası değerler (JSON; DELETE için null).</summary>
    public string? NewValuesJson { get; set; }
}
