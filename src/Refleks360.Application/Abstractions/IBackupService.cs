namespace Refleks360.Application.Abstractions;

public sealed record BackupInfo(string FileName, DateTime CreatedAtUtc, long SizeBytes);

public interface IBackupService
{
    /// <summary>BACKUP DATABASE çalıştırır; dosya yolu döner.</summary>
    Task<string> CreateBackupAsync(string targetFolder, CancellationToken ct = default);

    /// <summary>Belirtilen klasördeki tüm .bak dosyalarını listeler.</summary>
    Task<IReadOnlyList<BackupInfo>> ListBackupsAsync(string folder, CancellationToken ct = default);

    /// <summary>RESTORE DATABASE çalıştırır. DİKKAT: aktif bağlantıları keser.</summary>
    Task RestoreBackupAsync(string filePath, CancellationToken ct = default);
}
