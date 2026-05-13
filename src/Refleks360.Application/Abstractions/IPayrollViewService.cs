using Refleks360.Application.Payroll;

namespace Refleks360.Application.Abstractions;

public interface IPayrollViewService
{
    /// <summary>Tüm aktif çalışanların güncel ücret özet tablosu.</summary>
    Task<PayrollSnapshot> GetCurrentSnapshotAsync(CancellationToken ct = default);
}
