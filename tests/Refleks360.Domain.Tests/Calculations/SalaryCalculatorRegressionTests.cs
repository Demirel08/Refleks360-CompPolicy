using FluentAssertions;
using Refleks360.Domain.Calculations;

namespace Refleks360.Domain.Tests.Calculations;

/// <summary>
/// 50 brüt × 12 ay = 600 ay-vakalık altın referans (Python motoru) ile karşılaştırma.
/// Tüm tutarlar 1 kuruş (0.01 TL) toleransla doğrulanır — Python <c>float</c> vs C#
/// <c>decimal</c> aritmetiğinin doğal farkları için yeterli ve "kuruşa kadar" hedefini koruyan
/// sınır.
/// </summary>
public sealed class SalaryCalculatorRegressionTests
{
    private const decimal Tolerance = 0.01m;

    private static readonly RegressionFixture.Fixture _fixture = RegressionFixture.Load();
    private static readonly (TaxParameters Parameters, Func<int, MonthlyTaxPeriod> PeriodForMonth) _engine
        = RegressionFixture.BuildEngine(_fixture);

    public static TheoryData<int, decimal> Cases
    {
        get
        {
            var data = new TheoryData<int, decimal>();
            foreach (var c in _fixture.Cases)
            {
                data.Add(c.Id, (decimal)c.Gross);
            }
            return data;
        }
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void Calculator_matches_python_engine_within_one_kurus(int caseId, decimal grossInput)
    {
        var fixtureCase = _fixture.Cases.Single(c => c.Id == caseId);
        var gross = grossInput;

        decimal cumulativeBefore = 0m;
        foreach (var expected in fixtureCase.Months)
        {
            var period = _engine.PeriodForMonth(expected.Month);
            var actual = SalaryCalculator.CalculateMonth(gross, cumulativeBefore, period, _engine.Parameters);

            AssertClose(actual.Pek, expected.Pek, $"case#{caseId} ay{expected.Month} pek");
            AssertClose(actual.EmployeeContribution, expected.EmployeeContribution, $"case#{caseId} ay{expected.Month} emp_contrib");
            AssertClose(actual.MonthlyTaxBase, expected.MonthlyTaxBase, $"case#{caseId} ay{expected.Month} monthly_tax_base");
            AssertClose(actual.IncomeTaxRaw, expected.IncomeTaxRaw, $"case#{caseId} ay{expected.Month} gv_raw");
            AssertClose(actual.IncomeTaxExemption, expected.IncomeTaxExemption, $"case#{caseId} ay{expected.Month} gv_exemption");
            AssertClose(actual.IncomeTax, expected.IncomeTax, $"case#{caseId} ay{expected.Month} gv");
            AssertClose(actual.StampTax, expected.StampTax, $"case#{caseId} ay{expected.Month} stamp");
            AssertClose(actual.Net, expected.Net, $"case#{caseId} ay{expected.Month} net");
            AssertClose(actual.EmployerCost, expected.EmployerCost, $"case#{caseId} ay{expected.Month} employer_cost");

            cumulativeBefore += actual.MonthlyTaxBase;
        }
    }

    [Fact]
    public void Fixture_contains_exactly_fifty_cases()
    {
        _fixture.Cases.Should().HaveCount(50);
        _fixture.Cases.SelectMany(c => c.Months).Should().HaveCount(50 * 12);
    }

    private static void AssertClose(decimal actual, double expected, string what)
    {
        var expectedDecimal = (decimal)expected;
        var diff = Math.Abs(actual - expectedDecimal);
        diff.Should().BeLessThanOrEqualTo(Tolerance,
            $"{what}: actual={actual}, expected={expectedDecimal}, diff={diff}");
    }
}
