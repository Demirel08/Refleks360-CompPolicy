using Refleks360.Application.Audit;

namespace Refleks360.Application.Abstractions;

public interface IAuditQueryService
{
    Task<AuditPage> SearchAsync(AuditFilter filter, CancellationToken ct = default);
    Task<byte[]> ExportXlsxAsync(AuditFilter filter, CancellationToken ct = default);
}

public interface IKvkkService
{
    /// <summary>Çalışanı anonimleştir (kişisel veri sıfırla, sicil korunur).</summary>
    Task AnonymizeEmployeeAsync(int employeeId, string requestedBy, CancellationToken ct = default);

    /// <summary>Çalışanın tüm KVKK verilerini JSON olarak ihrac eder (right to access).</summary>
    Task<byte[]> ExportEmployeeDataAsync(int employeeId, CancellationToken ct = default);
}
