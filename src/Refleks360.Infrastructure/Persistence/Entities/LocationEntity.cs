namespace Refleks360.Infrastructure.Persistence.Entities;

/// <summary>Şirkete bağlı bir lokasyon (genel müdürlük, üretim tesisi, bölge ofisi).</summary>
public sealed class LocationEntity
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = "Türkiye";
    /// <summary>İstanbul = 100; diğer iller için %95, %85 gibi göreli endeks.</summary>
    public decimal RegionalIndexPercent { get; set; } = 100m;
    public bool IsActive { get; set; } = true;

    public CompanyEntity Company { get; set; } = default!;
}
