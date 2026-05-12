namespace Refleks360.Domain.Calculations;

/// <summary>
/// Bir vergi yılı için sabit oranlar ve dilimler. Aydan aya değişen
/// matrah/istisna değerleri <see cref="MonthlyTaxPeriod"/> içindedir.
/// </summary>
public sealed record TaxParameters
{
    /// <summary>SGK işçi prim oranı (örn. 0.14m = %14).</summary>
    public required decimal SgkEmployeeRate { get; init; }

    /// <summary>İşsizlik işçi prim oranı (örn. 0.01m = %1).</summary>
    public required decimal UnemploymentEmployeeRate { get; init; }

    /// <summary>SGK işveren prim oranı (örn. 0.2075m = %20.75).</summary>
    public required decimal SgkEmployerRate { get; init; }

    /// <summary>İşsizlik işveren prim oranı (örn. 0.02m = %2).</summary>
    public required decimal UnemploymentEmployerRate { get; init; }

    /// <summary>
    /// 5510 5/I-i teşvik vb. nedenlerle işveren SGK priminden düşülen oran
    /// (örn. 0.05m = %5). <see cref="ApplySgkEmployerDiscount"/> kapalıysa kullanılmaz.
    /// </summary>
    public required decimal SgkEmployerDiscountRate { get; init; }

    /// <summary>İşveren SGK indirimi uygulanacak mı?</summary>
    public required bool ApplySgkEmployerDiscount { get; init; }

    /// <summary>Damga vergisi oranı (örn. 0.00759m = binde 7,59).</summary>
    public required decimal StampTaxRate { get; init; }

    /// <summary>
    /// Gelir vergisi dilimleri (yıllık kümülatif matrah). Sıralı (artan üst sınır)
    /// olmalı; son dilim <see cref="decimal.MaxValue"/> ile kapatılır.
    /// </summary>
    public required IReadOnlyList<TaxBracket> IncomeTaxBrackets { get; init; }
}
