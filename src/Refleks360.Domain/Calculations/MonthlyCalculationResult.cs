namespace Refleks360.Domain.Calculations;

/// <summary>
/// Bir aylık brüt → net hesaplamasının tüm kalemleri. Python referansı:
/// <c>monthly_components_given_gross_and_cum</c> sözlüğü.
/// </summary>
/// <param name="Gross">Verilen aylık brüt.</param>
/// <param name="Pek">Prim Esasına Esas Kazanç (SGK matrahı; clip uygulanmış).</param>
/// <param name="EmployeeContribution">İşçi SGK + işsizlik primi toplamı.</param>
/// <param name="MonthlyTaxBase">Bu ayın vergi matrahı (brüt − işçi kesintileri).</param>
/// <param name="IncomeTaxRaw">İstisna uygulanmadan önce hesaplanan ham aylık GV.</param>
/// <param name="IncomeTaxExemption">Asgari ücret istisnası tutarı (GV indirimi).</param>
/// <param name="IncomeTax">İstisna sonrası ödenecek aylık GV.</param>
/// <param name="StampTax">İstisna sonrası ödenecek aylık damga vergisi.</param>
/// <param name="Net">Ele geçen aylık net.</param>
/// <param name="EmployerCost">İşverenin bu ay toplam yükü (brüt + işveren SGK + işveren işsizlik).</param>
public sealed record MonthlyCalculationResult(
    decimal Gross,
    decimal Pek,
    decimal EmployeeContribution,
    decimal MonthlyTaxBase,
    decimal IncomeTaxRaw,
    decimal IncomeTaxExemption,
    decimal IncomeTax,
    decimal StampTax,
    decimal Net,
    decimal EmployerCost);
