using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Refleks360.Application.Abstractions;
using Refleks360.Infrastructure.Persistence;

namespace Refleks360.Infrastructure.Services;

public sealed class BackupService(CompDbContext db, IConfiguration configuration) : IBackupService
{
    public async Task<string> CreateBackupAsync(string targetFolder, CancellationToken ct = default)
    {
        Directory.CreateDirectory(targetFolder);
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string yok.");

        var builder = new SqlConnectionStringBuilder(connectionString);
        var dbName = builder.InitialCatalog;
        var fileName = $"{dbName}_{DateTime.UtcNow:yyyyMMdd-HHmmss}.bak";
        var fullPath = Path.Combine(targetFolder, fileName);

        // BACKUP DATABASE
        await db.Database.ExecuteSqlAsync($"""
            BACKUP DATABASE [{dbName}]
            TO DISK = {fullPath}
            WITH FORMAT, INIT,
                 NAME = N'Refleks360-FullBackup', COMPRESSION;
            """, ct);

        return fullPath;
    }

    public Task<IReadOnlyList<BackupInfo>> ListBackupsAsync(string folder, CancellationToken ct = default)
    {
        if (!Directory.Exists(folder)) return Task.FromResult<IReadOnlyList<BackupInfo>>(Array.Empty<BackupInfo>());
        var list = Directory.EnumerateFiles(folder, "*.bak")
            .Select(p => new FileInfo(p))
            .OrderByDescending(f => f.LastWriteTimeUtc)
            .Select(f => new BackupInfo(f.FullName, f.LastWriteTimeUtc, f.Length))
            .ToList();
        return Task.FromResult<IReadOnlyList<BackupInfo>>(list);
    }

    public async Task RestoreBackupAsync(string filePath, CancellationToken ct = default)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string yok.");
        var builder = new SqlConnectionStringBuilder(connectionString);
        var dbName = builder.InitialCatalog;

        // Master DB üzerinden çalıştırılır (hedef DB'ye bağlı olmamamız gerekir)
        builder.InitialCatalog = "master";
        await using var conn = new SqlConnection(builder.ConnectionString);
        await conn.OpenAsync(ct);

        // 1) Single-user mode'a al (mevcut bağlantıları kapat)
        var setSingleUser = $"ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
        await using (var c1 = new SqlCommand(setSingleUser, conn)) await c1.ExecuteNonQueryAsync(ct);

        // 2) RESTORE
        var restore = $"RESTORE DATABASE [{dbName}] FROM DISK = N'{filePath.Replace("'", "''")}' WITH REPLACE;";
        await using (var c2 = new SqlCommand(restore, conn)) await c2.ExecuteNonQueryAsync(ct);

        // 3) Multi-user mode'a geri
        var setMultiUser = $"ALTER DATABASE [{dbName}] SET MULTI_USER;";
        await using (var c3 = new SqlCommand(setMultiUser, conn)) await c3.ExecuteNonQueryAsync(ct);
    }
}
