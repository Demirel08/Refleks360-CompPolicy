using Refleks360.Domain.Calculations;

namespace Refleks360.Application.Calculations;

/// <summary>
/// Bir vergi yılı için <see cref="SalaryCalculator"/> tarafından kullanılabilecek
/// hazır parametre paketi. Servis katmanı veritabanından okur, bu DTO'yu döner.
/// </summary>
/// <param name="Year">Yıl (örn. 2026).</param>
/// <param name="Parameters">Yıllık sabit oranlar + GV dilimleri.</param>
/// <param name="PeriodsByMonth">Ay (1..12) → ilgili <see cref="MonthlyTaxPeriod"/> eşlemesi.</param>
public sealed record YearTaxData(
    int Year,
    TaxParameters Parameters,
    IReadOnlyDictionary<int, MonthlyTaxPeriod> PeriodsByMonth);
