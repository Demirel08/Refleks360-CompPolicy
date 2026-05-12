using Refleks360.Application.Employees.Import;

namespace Refleks360.Application.Abstractions;

/// <summary>Excel toplu çalışan import (önizleme + uygulama).</summary>
public interface IEmployeeImportService
{
    /// <summary>Excel byte'larından satırları okur ve özet/hata listesi döner. DB'ye yazmaz.</summary>
    Task<EmployeeImportPreview> PreviewAsync(byte[] xlsxBytes, CancellationToken ct = default);

    /// <summary>Önizlemedeki tüm geçerli satırları uygular (yeni ekler veya günceller).</summary>
    Task<(int inserted, int updated)> ApplyAsync(byte[] xlsxBytes, CancellationToken ct = default);

    /// <summary>İndirilebilir boş şablon (xlsx byte).</summary>
    byte[] GenerateTemplate();
}
