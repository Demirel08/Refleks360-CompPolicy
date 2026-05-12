namespace Refleks360.Domain.Calculations;

/// <summary>
/// Yılın belirli aylarında geçerli olan SGK matrah, asgari ücret ve istisna parametreleri.
/// Spec: <c>docs/05-HESAPLAMA-MOTORU.md → "Yıl İçi Vergi Geçişleri"</c>.
/// </summary>
/// <param name="StartMonth">Bu dönemin geçerli olduğu ilk ay (1-12, dahil).</param>
/// <param name="EndMonth">Bu dönemin geçerli olduğu son ay (1-12, dahil).</param>
/// <param name="SgkBaseMin">SGK matrah taban (= aylık brüt asgari ücret).</param>
/// <param name="SgkBaseMax">SGK matrah tavan.</param>
/// <param name="GvExemptionAmount">
/// O ay için asgari ücretten elde edilen aylık matrah tutarı. Aylık vergi matrahından
/// bu kadarı, <see cref="GvExemptionRate"/> oranıyla GV indirimine girer.
/// </param>
/// <param name="GvExemptionRate">İstisna matrahına uygulanacak GV dilim oranı (örn. 0.15m).</param>
/// <param name="StampExemptionAmount">
/// O ay damga vergisinden istisna tutulan damga (brüt asgari ücret × damga oranı).
/// </param>
public sealed record MonthlyTaxPeriod(
    int StartMonth,
    int EndMonth,
    decimal SgkBaseMin,
    decimal SgkBaseMax,
    decimal GvExemptionAmount,
    decimal GvExemptionRate,
    decimal StampExemptionAmount)
{
    /// <summary>Asgari brüt ücret (SGK matrah tabanı ile aynı).</summary>
    public decimal MinWageGross => SgkBaseMin;
}
