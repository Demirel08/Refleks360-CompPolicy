namespace Refleks360.Domain.Calculations;

/// <summary>
/// Gelir vergisi yıllık kümülatif hesaplaması ve marjinal oran tespiti.
/// Saf bir hesaplayıcıdır — dış bağımlılığı yoktur.
/// Referans: <c>utils/calculations.py → annual_income_tax / find_marginal_rate</c>
/// (mevcut Python programı, Hafta 2 regression fixture'ın kaynağı).
/// </summary>
public static class IncomeTaxCalculator
{
    /// <summary>
    /// Verilen yıllık kümülatif matraha karşılık gelen toplam gelir vergisi.
    /// Dilim dilim ilerleyerek (limit − önceki limit) × oran şeklinde hesaplar.
    /// </summary>
    /// <param name="cumulativeTaxable">Yıl başından bu anki noktaya kadar olan kümülatif vergi matrahı.</param>
    /// <param name="brackets">Artan üst sınırla sıralanmış dilim listesi.</param>
    public static decimal AnnualTax(decimal cumulativeTaxable, IReadOnlyList<TaxBracket> brackets)
    {
        ArgumentNullException.ThrowIfNull(brackets);

        if (cumulativeTaxable <= 0m || brackets.Count == 0)
        {
            return 0m;
        }

        decimal tax = 0m;
        decimal prevLimit = 0m;

        foreach (var bracket in brackets)
        {
            decimal upper = bracket.UpperLimit;
            decimal take = Math.Min(cumulativeTaxable, upper) - prevLimit;
            if (take > 0m)
            {
                tax += take * bracket.Rate;
                prevLimit = upper;
            }

            if (cumulativeTaxable <= upper)
            {
                break;
            }
        }

        return tax;
    }

    /// <summary>
    /// Verilen kümülatif matraha karşılık gelen marjinal (içinde bulunulan) dilim oranı.
    /// Hiçbir dilime girmiyorsa son (en yüksek) dilimin oranı döner.
    /// </summary>
    public static decimal MarginalRate(decimal cumulativeTaxable, IReadOnlyList<TaxBracket> brackets)
    {
        ArgumentNullException.ThrowIfNull(brackets);

        if (brackets.Count == 0)
        {
            return 0m;
        }

        foreach (var bracket in brackets)
        {
            if (cumulativeTaxable <= bracket.UpperLimit)
            {
                return bracket.Rate;
            }
        }

        return brackets[^1].Rate;
    }
}
