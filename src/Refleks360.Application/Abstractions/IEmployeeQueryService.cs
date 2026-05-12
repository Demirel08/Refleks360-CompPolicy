using Refleks360.Application.Employees;

namespace Refleks360.Application.Abstractions;

/// <summary>
/// Çalışan liste sorguları için soyutlama. CRUD ve detay sorguları
/// MediatR handler'larına bölünecek (Hafta 5 sonu / Hafta 6 başı).
/// </summary>
public interface IEmployeeQueryService
{
    /// <summary>
    /// Tüm aktif (soft-delete edilmemiş) çalışanları liste DTO'su olarak döner.
    /// Sayfalama/sıralama/arama Syncfusion Grid tarafında istemci-side yapılır;
    /// 30 satır için bu yeterli. 1000+ çalışan için Hafta 7'de server-side moda geçilir.
    /// </summary>
    Task<IReadOnlyList<EmployeeListItem>> GetAllAsync(CancellationToken ct = default);
}
