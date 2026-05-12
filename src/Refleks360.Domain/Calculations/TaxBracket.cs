namespace Refleks360.Domain.Calculations;

/// <summary>
/// Bir gelir vergisi dilimi. Yıllık kümülatif matrah üst sınırı ve oranı tutar.
/// Dilimler sıralı (artan) bir koleksiyon olarak kullanılır; <see cref="UpperLimit"/>
/// son dilim için <see cref="decimal.MaxValue"/> verilir.
/// </summary>
/// <param name="UpperLimit">Yıllık kümülatif matrahın üst sınırı (TL).</param>
/// <param name="Rate">Bu dilimde uygulanan oran (örn. 0.15m = %15).</param>
public sealed record TaxBracket(decimal UpperLimit, decimal Rate);
