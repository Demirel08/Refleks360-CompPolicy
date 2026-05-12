namespace Refleks360.Infrastructure.Persistence.Entities;

/// <summary>
/// Yılın bir veya birkaç ayı için geçerli SGK matrah ve asgari ücret istisna parametreleri.
/// 12 aya tek tek satır olarak da, aynı parametreler için aralık olarak da saklanabilir.
/// </summary>
public sealed class MonthlyTaxPeriodEntity
{
    public int Id { get; set; }

    /// <summary>Foreign key → <see cref="TaxYearEntity.Year"/>.</summary>
    public int TaxYearId { get; set; }

    /// <summary>Bu dönemin geçerli olduğu ilk ay (1-12, dahil).</summary>
    public int StartMonth { get; set; }

    /// <summary>Bu dönemin geçerli olduğu son ay (1-12, dahil).</summary>
    public int EndMonth { get; set; }

    /// <summary>SGK matrah taban (= aylık brüt asgari ücret).</summary>
    public decimal SgkBaseMin { get; set; }

    /// <summary>SGK matrah tavan.</summary>
    public decimal SgkBaseMax { get; set; }

    /// <summary>Asgari ücret matrahından gelen istisna baz tutarı.</summary>
    public decimal GvExemptionAmount { get; set; }

    /// <summary>İstisna matrahına uygulanacak GV dilim oranı.</summary>
    public decimal GvExemptionRate { get; set; }

    /// <summary>Asgari ücret damga istisnası (aylık).</summary>
    public decimal StampExemptionAmount { get; set; }

    public TaxYearEntity TaxYear { get; set; } = default!;
}
