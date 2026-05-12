using Refleks360.Application.Calculations;

namespace Refleks360.Application.Abstractions;

/// <summary>
/// Vergi yılı parametrelerini sağlayan servis. Hesap motoru (Domain) sabit veri
/// taşımaz; UI ve uygulama katmanı bu servisten okur.
/// </summary>
public interface ITaxParameterService
{
    /// <summary>
    /// Verilen yıl için tüm parametreleri (oranlar + dilimler + 12 aylık dönem) döner.
    /// Yıl yoksa <see cref="InvalidOperationException"/> atar.
    /// </summary>
    Task<YearTaxData> GetForYearAsync(int year, CancellationToken ct = default);
}
