using FluentAssertions;
using Refleks360.Domain.Calculations;

namespace Refleks360.Domain.Tests.Calculations;

/// <summary>
/// Spec'teki <c>Test Vakaları</c> başlığındaki kritik kenar durumlar için hedefli testler.
/// Bu testler 2026 değerlerini doğrudan kullanır; regression fixture'a göre daha okunaklı
/// hata mesajı verir ve kod incelenirken neyin neden tuttuğunu açıklar.
/// </summary>
public sealed class SalaryCalculatorTargetedTests
{
    private const decimal MinWageGross2026 = 33_030.00m;
    private const decimal SgkBaseMin = MinWageGross2026;
    private const decimal SgkBaseMax = MinWageGross2026 * 7.5m;             // 247.725,00
    private const decimal GvExemptionBase = 28_075.50m;                     // brut - SGK işçi
    private const decimal StampExemptionPerMonth = MinWageGross2026 * 0.00759m; // 250,6977

    private static readonly IReadOnlyList<TaxBracket> _brackets = new[]
    {
        new TaxBracket(190_000m, 0.15m),
        new TaxBracket(400_000m, 0.20m),
        new TaxBracket(1_500_000m, 0.27m),
        new TaxBracket(5_300_000m, 0.35m),
        new TaxBracket(decimal.MaxValue, 0.40m),
    };

    private static readonly TaxParameters _params = new()
    {
        SgkEmployeeRate = 0.14m,
        UnemploymentEmployeeRate = 0.01m,
        SgkEmployerRate = 0.2075m,
        UnemploymentEmployerRate = 0.02m,
        SgkEmployerDiscountRate = 0.05m,
        ApplySgkEmployerDiscount = false,
        StampTaxRate = 0.00759m,
        IncomeTaxBrackets = _brackets,
    };

    private static MonthlyTaxPeriod PeriodForMonth(int month, decimal exemptionRate) => new(
        StartMonth: month,
        EndMonth: month,
        SgkBaseMin: SgkBaseMin,
        SgkBaseMax: SgkBaseMax,
        GvExemptionAmount: GvExemptionBase,
        GvExemptionRate: exemptionRate,
        StampExemptionAmount: StampExemptionPerMonth);

    /// <summary>
    /// Spec 1: Brüt asgari ücret 33.030, net asgari 28.075,50 — kuruşa kadar tutmalı.
    /// </summary>
    [Fact]
    public void MinimumWageEmployee_ProducesExactNetMinimumWage()
    {
        var period = PeriodForMonth(month: 1, exemptionRate: 0.15m);
        var result = SalaryCalculator.CalculateMonth(MinWageGross2026, 0m, period, _params);

        result.Pek.Should().Be(MinWageGross2026);
        result.EmployeeContribution.Should().Be(MinWageGross2026 * 0.15m); // SGK %14 + işsizlik %1
        result.MonthlyTaxBase.Should().Be(28_075.50m);
        result.IncomeTax.Should().Be(0m, "asgari ücret istisnası tüm aylık GV'yi karşılar");
        result.StampTax.Should().Be(0m, "asgari ücret damga istisnası tüm damgayı karşılar");
        result.Net.Should().Be(28_075.50m);
    }

    /// <summary>
    /// Spec 3: 250.000 brüt → SGK matrah max 247.725'a clip — PEK tavanda kalır.
    /// </summary>
    [Fact]
    public void HighSalary_AboveSgkCeiling_ClipsPekAtMax()
    {
        const decimal gross = 250_000m;
        var period = PeriodForMonth(month: 1, exemptionRate: 0.15m);
        var result = SalaryCalculator.CalculateMonth(gross, 0m, period, _params);

        result.Pek.Should().Be(SgkBaseMax);
        result.EmployeeContribution.Should().Be(SgkBaseMax * 0.15m);
    }

    /// <summary>
    /// Spec 6: SGK işveren teşviki açık → işveren SGK %5 indirim alır.
    /// </summary>
    [Fact]
    public void EmployerDiscount_When_Enabled_Reduces_EmployerCost()
    {
        const decimal gross = 60_000m;
        var period = PeriodForMonth(month: 1, exemptionRate: 0.15m);

        var withoutDiscount = SalaryCalculator.CalculateMonth(gross, 0m, period, _params);
        var withDiscount = SalaryCalculator.CalculateMonth(gross, 0m, period,
            _params with { ApplySgkEmployerDiscount = true });

        withDiscount.EmployerCost.Should().BeLessThan(withoutDiscount.EmployerCost);

        // Tasarruf: pek × sgkEmployerRate × discountRate = 60.000 × 0.2075 × 0.05
        var expectedSavings = gross * 0.2075m * 0.05m;
        (withoutDiscount.EmployerCost - withDiscount.EmployerCost)
            .Should().Be(expectedSavings);
    }

    /// <summary>
    /// Spec 4: Yıl içinde dilim geçişi — Temmuz/Ağustos sınırında GV artar.
    /// 60K brüt için Ocak ayı vs Ağustos ayı (kümülatif matrah dilim 2'ye geçtikten sonra) GV farklı olur.
    /// </summary>
    [Fact]
    public void YearProgression_TaxBracketCrossing_IncreasesIncomeTax()
    {
        const decimal gross = 60_000m;
        decimal cumulative = 0m;
        var marchPeriod = PeriodForMonth(month: 3, exemptionRate: 0.15m);
        var monthlyResults = new List<MonthlyCalculationResult>();

        for (int m = 1; m <= 12; m++)
        {
            var rate = m <= 7 ? 0.15m : 0.20m;
            var p = PeriodForMonth(m, rate);
            var r = SalaryCalculator.CalculateMonth(gross, cumulative, p, _params);
            monthlyResults.Add(r);
            cumulative += r.MonthlyTaxBase;
        }

        // 60K brut × 12 ay × 0.85 (1 - kesintiler) ≈ 612.000 → ikinci dilime girer (400K geç)
        cumulative.Should().BeGreaterThan(400_000m);
        // Kasım ya da Aralık'taki GV, Ocak'tan büyük olmalı
        monthlyResults[11].IncomeTax.Should().BeGreaterThan(monthlyResults[0].IncomeTax);
    }

    /// <summary>
    /// Net asgari ücretten az brüt verilse bile, PEK SGK tabanına yükselir (clip min).
    /// </summary>
    [Fact]
    public void BelowMinWage_Gross_ClipsPekAtSgkMin()
    {
        const decimal gross = 25_000m;
        var period = PeriodForMonth(month: 1, exemptionRate: 0.15m);
        var result = SalaryCalculator.CalculateMonth(gross, 0m, period, _params);

        result.Pek.Should().Be(SgkBaseMin);
        // PEK > gross olduğu için işçi kesintileri PEK üzerinden hesaplanır;
        // dolayısıyla net negatife düşebilir. Bu test sadece clip mantığını doğrular,
        // iş kuralı (asgari ücret altı çalışan olmamalı) UI/validation katmanında olur.
        result.MonthlyTaxBase.Should().BeLessThan(gross);
    }

    /// <summary>
    /// Yıllık simülasyon: aynı brütle 12 ay, kümülatif artışıyla ortalama net aylar arasında değişiyor.
    /// </summary>
    [Fact]
    public void SimulateYearStableGross_TracksCumulativeTaxBaseAcrossMonths()
    {
        decimal gross = 100_000m;
        var results = SalaryCalculator.SimulateYearStableGross(
            gross,
            m => PeriodForMonth(m, m <= 7 ? 0.15m : 0.20m),
            _params);

        results.Should().HaveCount(12);

        // Yıl ilerledikçe kümülatif vergi matrahı artar → bazı aylar GV artar
        var totalIncomeTax = results.Sum(r => r.IncomeTax);
        totalIncomeTax.Should().BeGreaterThan(0m);
    }
}
