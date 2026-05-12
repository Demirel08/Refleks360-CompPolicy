using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Abstractions;
using Refleks360.Application.Audit;
using Refleks360.Infrastructure.Persistence;
using Syncfusion.XlsIO;

namespace Refleks360.Infrastructure.Services;

public sealed class AuditQueryService(CompDbContext db) : IAuditQueryService
{
    public async Task<AuditPage> SearchAsync(AuditFilter filter, CancellationToken ct = default)
    {
        var q = db.AuditLogs.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter.UserName)) q = q.Where(a => a.UserName == filter.UserName);
        if (!string.IsNullOrWhiteSpace(filter.EntityType)) q = q.Where(a => a.EntityType == filter.EntityType);
        if (!string.IsNullOrWhiteSpace(filter.EntityKey)) q = q.Where(a => a.EntityKey == filter.EntityKey);
        if (filter.FromUtc.HasValue) q = q.Where(a => a.TimestampUtc >= filter.FromUtc);
        if (filter.ToUtc.HasValue) q = q.Where(a => a.TimestampUtc <= filter.ToUtc);

        int total = await q.CountAsync(ct);
        var rows = await q.OrderByDescending(a => a.TimestampUtc)
            .Skip(filter.Skip).Take(filter.Take)
            .Select(a => new AuditEntryDto(a.Id, a.TimestampUtc, a.UserName, a.Action, a.EntityType, a.EntityKey, a.OldValuesJson, a.NewValuesJson))
            .ToListAsync(ct);

        return new AuditPage(rows, total);
    }

    public async Task<byte[]> ExportXlsxAsync(AuditFilter filter, CancellationToken ct = default)
    {
        // Limitsiz export — bu izin kontrolü UI tarafında SystemAdmin/Auditor için
        var q = db.AuditLogs.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter.UserName)) q = q.Where(a => a.UserName == filter.UserName);
        if (!string.IsNullOrWhiteSpace(filter.EntityType)) q = q.Where(a => a.EntityType == filter.EntityType);
        if (!string.IsNullOrWhiteSpace(filter.EntityKey)) q = q.Where(a => a.EntityKey == filter.EntityKey);
        if (filter.FromUtc.HasValue) q = q.Where(a => a.TimestampUtc >= filter.FromUtc);
        if (filter.ToUtc.HasValue) q = q.Where(a => a.TimestampUtc <= filter.ToUtc);

        var rows = await q.OrderByDescending(a => a.TimestampUtc).ToListAsync(ct);

        using var engine = new ExcelEngine();
        engine.Excel.DefaultVersion = ExcelVersion.Xlsx;
        var book = engine.Excel.Workbooks.Create(1);
        var s = book.Worksheets[0];
        s.Name = "AuditLog";

        string[] headers = { "Tarih (UTC)", "Kullanıcı", "İşlem", "Entity", "Anahtar", "Eski", "Yeni" };
        for (int i = 0; i < headers.Length; i++) s.SetText(1, i + 1, headers[i]);
        s.Range[1, 1, 1, headers.Length].CellStyle.Font.Bold = true;

        int r = 2;
        foreach (var a in rows)
        {
            s[r, 1].DateTime = a.TimestampUtc;
            s[r, 1].NumberFormat = "dd.MM.yyyy HH:mm:ss";
            s.SetText(r, 2, a.UserName);
            s.SetText(r, 3, a.Action);
            s.SetText(r, 4, a.EntityType);
            s.SetText(r, 5, a.EntityKey);
            s.SetText(r, 6, a.OldValuesJson);
            s.SetText(r, 7, a.NewValuesJson);
            r++;
        }
        s.UsedRange.AutofitColumns();
        using var ms = new MemoryStream();
        book.SaveAs(ms);
        return ms.ToArray();
    }
}

public sealed class KvkkService(CompDbContext db) : IKvkkService
{
    public async Task AnonymizeEmployeeAsync(int employeeId, string requestedBy, CancellationToken ct = default)
    {
        var e = await db.Employees.FirstAsync(x => x.Id == employeeId, ct);
        e.FirstName = "ANONIM";
        e.LastName = "ANONIM";
        e.Email = null;
        e.Phone = null;
        e.Notes = $"KVKK silme talebi: {requestedBy} tarafından {DateTime.UtcNow:u}";
        await db.SaveChangesAsync(ct);
    }

    public async Task<byte[]> ExportEmployeeDataAsync(int employeeId, CancellationToken ct = default)
    {
        var e = await db.Employees.AsNoTracking()
            .Include(x => x.Department)
            .Include(x => x.Position)
            .Include(x => x.Location)
            .FirstAsync(x => x.Id == employeeId, ct);
        var salaries = await db.EmployeeSalaries.AsNoTracking()
            .Where(s => s.EmployeeId == employeeId).ToListAsync(ct);

        var data = new
        {
            Employee = new { e.Id, e.EmployeeNumber, e.FirstName, e.LastName, e.Email, e.Phone, e.HireDate, e.TerminationDate, Department = e.Department.Name, Position = e.Position.Title, Location = e.Location.Name },
            Salaries = salaries.Select(s => new { s.Id, s.GrossMonthly, s.EffectiveDate, s.EndDate, s.Reason, s.CreatedAtUtc }),
            ExportedAt = DateTime.UtcNow,
        };
        return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(data, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
        });
    }
}
