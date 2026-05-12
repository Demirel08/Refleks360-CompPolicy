namespace Refleks360.Infrastructure.Persistence.Entities;

/// <summary>
/// Bir vergi yılı için aydan aya değişmeyen sabit oranlar.
/// İlgili <see cref="IncomeTaxBracketEntity"/> ve <see cref="MonthlyTaxPeriodEntity"/>
/// kayıtları aynı <see cref="Year"/> üzerinden eşleşir.
/// </summary>
public sealed class TaxYearEntity
{
    /// <summary>Yıl (örn. 2026). Birincil anahtar.</summary>
    public int Year { get; set; }

    /// <summary>SGK işçi prim oranı (örn. 0.14m = %14).</summary>
    public decimal SgkEmployeeRate { get; set; }

    /// <summary>İşsizlik işçi prim oranı.</summary>
    public decimal UnemploymentEmployeeRate { get; set; }

    /// <summary>SGK işveren prim oranı.</summary>
    public decimal SgkEmployerRate { get; set; }

    /// <summary>İşsizlik işveren prim oranı.</summary>
    public decimal UnemploymentEmployerRate { get; set; }

    /// <summary>5510 5/I-i vb. teşvik indirim oranı.</summary>
    public decimal SgkEmployerDiscountRate { get; set; }

    /// <summary>Bu yıl için varsayılan olarak SGK işveren teşviki uygulansın mı?</summary>
    public bool ApplySgkEmployerDiscount { get; set; }

    /// <summary>Damga vergisi oranı.</summary>
    public decimal StampTaxRate { get; set; }

    public List<IncomeTaxBracketEntity> IncomeTaxBrackets { get; set; } = new();
    public List<MonthlyTaxPeriodEntity> MonthlyPeriods { get; set; } = new();
}
