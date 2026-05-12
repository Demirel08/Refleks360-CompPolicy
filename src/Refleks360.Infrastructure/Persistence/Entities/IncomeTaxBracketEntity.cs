namespace Refleks360.Infrastructure.Persistence.Entities;

/// <summary>
/// Bir vergi yılına ait gelir vergisi dilimi.
/// Sıralama <see cref="OrderIndex"/> ile garanti edilir (artan).
/// </summary>
public sealed class IncomeTaxBracketEntity
{
    public int Id { get; set; }

    /// <summary>Foreign key → <see cref="TaxYearEntity.Year"/>.</summary>
    public int TaxYearId { get; set; }

    /// <summary>Dilim sırası (0-based, küçük olan altta).</summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// Yıllık kümülatif matrah üst sınırı. Son dilim için
    /// <see cref="decimal.MaxValue"/>'a yakın bir değer kullanılır
    /// (sproc sınırlaması yok, decimal(18,2) kolonda saklanır).
    /// </summary>
    public decimal UpperLimit { get; set; }

    /// <summary>Dilim oranı (örn. 0.15m = %15).</summary>
    public decimal Rate { get; set; }

    public TaxYearEntity TaxYear { get; set; } = default!;
}
