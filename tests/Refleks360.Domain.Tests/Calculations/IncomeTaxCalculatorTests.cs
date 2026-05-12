using FluentAssertions;
using Refleks360.Domain.Calculations;

namespace Refleks360.Domain.Tests.Calculations;

/// <summary>
/// <see cref="IncomeTaxCalculator"/> için hedefli birim testler. Regresyon fixture'ı
/// uçtan uca doğrulamayı kapsar; bu testler dilim sınırlarındaki davranışı kuru
/// (parametre tablosuz) doğrular.
/// </summary>
public sealed class IncomeTaxCalculatorTests
{
    private static readonly IReadOnlyList<TaxBracket> _brackets2026 = new[]
    {
        new TaxBracket(190_000m, 0.15m),
        new TaxBracket(400_000m, 0.20m),
        new TaxBracket(1_500_000m, 0.27m),
        new TaxBracket(5_300_000m, 0.35m),
        new TaxBracket(decimal.MaxValue, 0.40m),
    };

    [Theory]
    [InlineData(0, 0)]
    [InlineData(100_000, 15_000)]               // tamamen %15 dilimi
    [InlineData(190_000, 28_500)]               // tam ilk dilim sınırı
    [InlineData(200_000, 28_500 + 2_000)]       // ilk dilim doldu + 10K × %20
    [InlineData(400_000, 28_500 + 42_000)]      // ikinci dilim tam (210K × %20)
    [InlineData(1_500_000, 70_500 + 297_000)]   // üçüncü dilim tam (1.1M × %27)
    [InlineData(5_300_000, 367_500 + 1_330_000)]// dördüncü dilim tam (3.8M × %35)
    [InlineData(6_000_000, 367_500 + 1_330_000 + 280_000)] // beşinci dilime taşma (700K × %40)
    public void AnnualTax_returns_expected_for_2026_ucret_brackets(decimal cumulativeTaxable, decimal expected)
    {
        var actual = IncomeTaxCalculator.AnnualTax(cumulativeTaxable, _brackets2026);
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(50_000, 0.15)]
    [InlineData(190_000, 0.15)]   // dilim sınırı kendi dilimine ait
    [InlineData(190_001, 0.20)]
    [InlineData(400_000, 0.20)]
    [InlineData(1_500_000.01, 0.35)]
    [InlineData(10_000_000, 0.40)]
    public void MarginalRate_resolves_correct_bracket(decimal cumulativeTaxable, double expectedRate)
    {
        var actual = IncomeTaxCalculator.MarginalRate(cumulativeTaxable, _brackets2026);
        actual.Should().Be((decimal)expectedRate);
    }

    [Fact]
    public void AnnualTax_with_zero_brackets_returns_zero()
    {
        var actual = IncomeTaxCalculator.AnnualTax(100_000m, Array.Empty<TaxBracket>());
        actual.Should().Be(0m);
    }

    [Fact]
    public void AnnualTax_with_null_brackets_throws()
    {
        Action act = () => IncomeTaxCalculator.AnnualTax(100_000m, null!);
        act.Should().Throw<ArgumentNullException>();
    }
}
