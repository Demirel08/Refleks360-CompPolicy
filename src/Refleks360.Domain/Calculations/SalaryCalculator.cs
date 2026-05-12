namespace Refleks360.Domain.Calculations;

/// <summary>
/// Aylık brüt → net dönüşümü ve işveren maliyeti. Spec:
/// <c>docs/05-HESAPLAMA-MOTORU.md → "Net Hesaplama Algoritması"</c>.
/// Python referansı: <c>utils/calculations.py → monthly_components_given_gross_and_cum</c>.
/// </summary>
/// <remarks>
/// Tüm iç hesaplamalar <see cref="decimal"/> üzerinde, Python'daki <c>float</c> davranışıyla
/// birebir aynı sıra ve operatörlerle yapılır. Bu sınıf saf C# — hiçbir DI yok, IO yok.
/// </remarks>
public static class SalaryCalculator
{
    /// <summary>
    /// Bir ayın tüm hesap kalemlerini üretir.
    /// </summary>
    /// <param name="gross">Verilen aylık brüt (TL).</param>
    /// <param name="cumulativeTaxablePrev">Yıl başından bu aydan ÖNCE birikmiş vergi matrahı.</param>
    /// <param name="period">O ayın geçerli SGK matrah/asgari ücret istisna parametreleri.</param>
    /// <param name="parameters">Yıllık sabit oranlar + GV dilimleri.</param>
    public static MonthlyCalculationResult CalculateMonth(
        decimal gross,
        decimal cumulativeTaxablePrev,
        MonthlyTaxPeriod period,
        TaxParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(period);
        ArgumentNullException.ThrowIfNull(parameters);

        // 1. PEK (Prim Esasına Esas Kazanç): brüt'ü SGK matrah min/max'a clip
        decimal pek = ClipPek(gross, period.SgkBaseMin, period.SgkBaseMax);

        // 2-4. İşçi kesintileri
        decimal employeeSgk = pek * parameters.SgkEmployeeRate;
        decimal employeeUnemp = pek * parameters.UnemploymentEmployeeRate;
        decimal employeeContribution = employeeSgk + employeeUnemp;

        // 5. Aylık vergi matrahı
        decimal monthlyTaxBase = Math.Max(0m, gross - employeeContribution);

        // 6-9. GV: yıllık kümülatif farkı
        decimal cumulativeAfter = cumulativeTaxablePrev + monthlyTaxBase;
        decimal annualNew = IncomeTaxCalculator.AnnualTax(cumulativeAfter, parameters.IncomeTaxBrackets);
        decimal annualOld = IncomeTaxCalculator.AnnualTax(cumulativeTaxablePrev, parameters.IncomeTaxBrackets);
        decimal incomeTaxRaw = Math.Max(0m, annualNew - annualOld);

        // 10-12. Asgari ücret GV istisnası
        decimal exemptionBase = Math.Min(period.GvExemptionAmount, monthlyTaxBase);
        decimal incomeTaxExemption = exemptionBase * period.GvExemptionRate;
        decimal incomeTax = Math.Max(0m, incomeTaxRaw - incomeTaxExemption);

        // 13-15. Damga + istisnası
        decimal stampRaw = gross * parameters.StampTaxRate;
        decimal stampExemption = Math.Min(period.StampExemptionAmount, stampRaw);
        decimal stampTax = Math.Max(0m, stampRaw - stampExemption);

        // 16. Net
        decimal net = gross - employeeContribution - incomeTax - stampTax;

        // 17-20. İşveren maliyeti
        decimal employerSgkRaw = pek * parameters.SgkEmployerRate;
        decimal employerUnemp = pek * parameters.UnemploymentEmployerRate;
        decimal employerSgk = parameters.ApplySgkEmployerDiscount
            ? employerSgkRaw * (1m - parameters.SgkEmployerDiscountRate)
            : employerSgkRaw;
        decimal employerCost = gross + employerSgk + employerUnemp;

        return new MonthlyCalculationResult(
            Gross: gross,
            Pek: pek,
            EmployeeContribution: employeeContribution,
            MonthlyTaxBase: monthlyTaxBase,
            IncomeTaxRaw: incomeTaxRaw,
            IncomeTaxExemption: incomeTaxExemption,
            IncomeTax: incomeTax,
            StampTax: stampTax,
            Net: net,
            EmployerCost: employerCost);
    }

    /// <summary>
    /// 12 ay boyunca aynı brüt ile yıllık simülasyon (kümülatif vergi matrahını taşır).
    /// Python referansı: <c>simulate_year(mode="stable_gross")</c>.
    /// </summary>
    /// <param name="gross">Tüm ay aynı brüt (TL).</param>
    /// <param name="periodForMonth">Verilen ay numarasına (1-12) göre <see cref="MonthlyTaxPeriod"/> döndüren fonksiyon.</param>
    /// <param name="parameters">Yıllık sabit parametreler.</param>
    /// <returns>12 aylık sonuç dizisi (Ocak → Aralık).</returns>
    public static IReadOnlyList<MonthlyCalculationResult> SimulateYearStableGross(
        decimal gross,
        Func<int, MonthlyTaxPeriod> periodForMonth,
        TaxParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(periodForMonth);
        ArgumentNullException.ThrowIfNull(parameters);

        var results = new MonthlyCalculationResult[12];
        decimal cumulative = 0m;

        for (int m = 1; m <= 12; m++)
        {
            var period = periodForMonth(m);
            var result = CalculateMonth(gross, cumulative, period, parameters);
            results[m - 1] = result;
            cumulative += result.MonthlyTaxBase;
        }

        return results;
    }

    /// <summary>
    /// PEK clip: brüt değer SGK matrah min ile max arasına sıkıştırılır.
    /// </summary>
    private static decimal ClipPek(decimal gross, decimal minBase, decimal maxBase)
    {
        if (gross < minBase) return minBase;
        if (gross > maxBase) return maxBase;
        return gross;
    }
}
