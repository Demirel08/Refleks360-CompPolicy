using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Abstractions;
using Refleks360.Application.Employees.Import;
using Refleks360.Domain.Organization;
using Refleks360.Infrastructure.Persistence;
using Refleks360.Infrastructure.Persistence.Entities;
using Syncfusion.XlsIO;

namespace Refleks360.Infrastructure.Services;

/// <summary>
/// Excel toplu çalışan import (Syncfusion XlsIO). 17 standart başlıkla beklenir:
/// Sicil, Ad, Soyad, E-posta, Telefon, Cinsiyet, Sözleşme, Çalışma, İşe Giriş,
/// Pozisyon, Departman, Lokasyon, Notlar.
/// </summary>
public sealed class EmployeeImportService(CompDbContext db) : IEmployeeImportService
{
    private static readonly string[] Headers = new[]
    {
        "Sicil", "Ad", "Soyad", "E-posta", "Telefon",
        "Cinsiyet", "Sözleşme", "Çalışma",
        "İşe Giriş", "Pozisyon", "Departman", "Lokasyon", "Notlar"
    };

    public byte[] GenerateTemplate()
    {
        using var engine = new ExcelEngine();
        var app = engine.Excel;
        app.DefaultVersion = ExcelVersion.Xlsx;

        var book = app.Workbooks.Create(1);
        var sheet = book.Worksheets[0];
        sheet.Name = "Çalışanlar";

        for (int i = 0; i < Headers.Length; i++)
        {
            sheet.SetText(1, i + 1, Headers[i]);
        }
        sheet.Range[1, 1, 1, Headers.Length].CellStyle.Font.Bold = true;

        // Örnek satır
        sheet.SetText(2, 1, "T1001");
        sheet.SetText(2, 2, "Örnek");
        sheet.SetText(2, 3, "Çalışan");
        sheet.SetText(2, 4, "ornek@firma.com");
        sheet.SetText(2, 5, "5551234567");
        sheet.SetText(2, 6, "Erkek");
        sheet.SetText(2, 7, "Süresiz");
        sheet.SetText(2, 8, "Tam zamanlı");
        sheet[2, 9].DateTime = new DateTime(2026, 1, 15);
        sheet[2, 9].NumberFormat = "dd.MM.yyyy";
        sheet.SetText(2, 10, "Yazılım Geliştirme Mühendisi");
        sheet.SetText(2, 11, "Bilgi Teknolojileri");
        sheet.SetText(2, 12, "İstanbul Genel Müdürlük");
        sheet.SetText(2, 13, "Açıklama");

        sheet.UsedRange.AutofitColumns();

        using var ms = new MemoryStream();
        book.SaveAs(ms);
        return ms.ToArray();
    }

    public async Task<EmployeeImportPreview> PreviewAsync(byte[] xlsxBytes, CancellationToken ct = default)
    {
        var rows = ReadRows(xlsxBytes);
        var (positions, departments, locations) = await LoadLookupsAsync(ct);
        var existingNumbers = await db.Employees.Select(e => e.EmployeeNumber).ToListAsync(ct);
        var existingSet = new HashSet<string>(existingNumbers, StringComparer.OrdinalIgnoreCase);

        var results = new List<EmployeeImportRowResult>(rows.Count);
        int valid = 0, invalid = 0, inserts = 0, updates = 0;

        foreach (var r in rows)
        {
            var errors = ValidateRow(r, positions, departments, locations);
            var isNew = !existingSet.Contains(r.EmployeeNumber);
            var isValid = errors.Count == 0;
            if (isValid)
            {
                valid++;
                if (isNew) inserts++; else updates++;
            }
            else invalid++;

            results.Add(new EmployeeImportRowResult(
                r.RowNumber,
                r.EmployeeNumber,
                $"{r.FirstName} {r.LastName}".Trim(),
                isNew,
                isValid,
                errors));
        }

        return new EmployeeImportPreview(rows.Count, valid, invalid, inserts, updates, results);
    }

    public async Task<(int inserted, int updated)> ApplyAsync(byte[] xlsxBytes, CancellationToken ct = default)
    {
        var rows = ReadRows(xlsxBytes);
        var (positions, departments, locations) = await LoadLookupsAsync(ct);
        var existing = await db.Employees.ToListAsync(ct);
        var byNumber = existing.ToDictionary(e => e.EmployeeNumber, StringComparer.OrdinalIgnoreCase);

        int inserted = 0, updated = 0;

        foreach (var r in rows)
        {
            var errors = ValidateRow(r, positions, departments, locations);
            if (errors.Count > 0) continue;

            var posId = positions[r.PositionTitle];
            var deptId = departments[r.DepartmentName];
            var locId = locations[r.LocationName];
            var gender = ParseGender(r.GenderText);
            var emp = ParseEmployment(r.EmploymentTypeText);
            var ws = ParseSchedule(r.WorkScheduleText);

            if (byNumber.TryGetValue(r.EmployeeNumber, out var entity))
            {
                entity.FirstName = r.FirstName;
                entity.LastName = r.LastName;
                entity.Email = r.Email;
                entity.Phone = r.Phone;
                entity.Gender = gender;
                entity.EmploymentType = emp;
                entity.WorkSchedule = ws;
                entity.HireDate = r.HireDate!.Value;
                entity.PositionId = posId;
                entity.DepartmentId = deptId;
                entity.LocationId = locId;
                entity.Notes = r.Notes;
                updated++;
            }
            else
            {
                db.Employees.Add(new EmployeeEntity
                {
                    EmployeeNumber = r.EmployeeNumber,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    Email = r.Email,
                    Phone = r.Phone,
                    Gender = gender,
                    EmploymentType = emp,
                    WorkSchedule = ws,
                    Status = EmployeeStatus.Active,
                    HireDate = r.HireDate!.Value,
                    PositionId = posId,
                    DepartmentId = deptId,
                    LocationId = locId,
                    Notes = r.Notes,
                    IsLocked = false,
                    IsDeleted = false,
                });
                inserted++;
            }
        }

        await db.SaveChangesAsync(ct);
        return (inserted, updated);
    }

    // --- Lookup ve doğrulama ---

    private async Task<(Dictionary<string, int> positions, Dictionary<string, int> departments, Dictionary<string, int> locations)>
        LoadLookupsAsync(CancellationToken ct)
    {
        var positions = await db.Positions.AsNoTracking()
            .ToDictionaryAsync(p => p.Title, p => p.Id, StringComparer.OrdinalIgnoreCase, ct);
        var departments = await db.Departments.AsNoTracking()
            .ToDictionaryAsync(d => d.Name, d => d.Id, StringComparer.OrdinalIgnoreCase, ct);
        var locations = await db.Locations.AsNoTracking()
            .ToDictionaryAsync(l => l.Name, l => l.Id, StringComparer.OrdinalIgnoreCase, ct);
        return (positions, departments, locations);
    }

    private static List<string> ValidateRow(
        EmployeeImportRow r,
        Dictionary<string, int> positions,
        Dictionary<string, int> departments,
        Dictionary<string, int> locations)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(r.EmployeeNumber)) errors.Add("Sicil zorunlu");
        if (string.IsNullOrWhiteSpace(r.FirstName)) errors.Add("Ad zorunlu");
        if (string.IsNullOrWhiteSpace(r.LastName)) errors.Add("Soyad zorunlu");
        if (r.HireDate is null) errors.Add("İşe Giriş geçersiz");
        if (!positions.ContainsKey(r.PositionTitle)) errors.Add($"Pozisyon bulunamadı: {r.PositionTitle}");
        if (!departments.ContainsKey(r.DepartmentName)) errors.Add($"Departman bulunamadı: {r.DepartmentName}");
        if (!locations.ContainsKey(r.LocationName)) errors.Add($"Lokasyon bulunamadı: {r.LocationName}");
        if (!string.IsNullOrWhiteSpace(r.Email) && !r.Email.Contains('@')) errors.Add("E-posta geçersiz");
        return errors;
    }

    private static Gender ParseGender(string text) => text.Trim().ToLowerInvariant() switch
    {
        "erkek" or "male" or "e" => Gender.Male,
        "kadın" or "kadin" or "female" or "k" => Gender.Female,
        _ => Gender.NotSpecified,
    };

    private static EmploymentType ParseEmployment(string text) => text.Trim().ToLowerInvariant() switch
    {
        "süresiz" or "suresiz" or "permanent" or "kadrolu" => EmploymentType.Permanent,
        "belirli süreli" or "fixed-term" or "fixed" or "fixed term" => EmploymentType.FixedTerm,
        "stajyer" or "intern" => EmploymentType.Intern,
        _ => EmploymentType.Permanent,
    };

    private static WorkSchedule ParseSchedule(string text) => text.Trim().ToLowerInvariant() switch
    {
        "yarı zamanlı" or "yari zamanli" or "part-time" or "parttime" => WorkSchedule.PartTime,
        _ => WorkSchedule.FullTime,
    };

    private List<EmployeeImportRow> ReadRows(byte[] bytes)
    {
        using var engine = new ExcelEngine();
        var app = engine.Excel;
        app.DefaultVersion = ExcelVersion.Xlsx;
        using var ms = new MemoryStream(bytes);
        var book = app.Workbooks.Open(ms);
        var sheet = book.Worksheets[0];

        var rows = new List<EmployeeImportRow>();
        int lastRow = sheet.UsedRange.LastRow;

        for (int r = 2; r <= lastRow; r++)
        {
            var row = new EmployeeImportRow
            {
                RowNumber = r,
                EmployeeNumber = sheet[r, 1].Value?.Trim() ?? string.Empty,
                FirstName = sheet[r, 2].Value?.Trim() ?? string.Empty,
                LastName = sheet[r, 3].Value?.Trim() ?? string.Empty,
                Email = NullIfEmpty(sheet[r, 4].Value?.Trim()),
                Phone = NullIfEmpty(sheet[r, 5].Value?.Trim()),
                GenderText = sheet[r, 6].Value?.Trim() ?? string.Empty,
                EmploymentTypeText = sheet[r, 7].Value?.Trim() ?? string.Empty,
                WorkScheduleText = sheet[r, 8].Value?.Trim() ?? string.Empty,
                HireDate = ReadDate(sheet[r, 9]),
                PositionTitle = sheet[r, 10].Value?.Trim() ?? string.Empty,
                DepartmentName = sheet[r, 11].Value?.Trim() ?? string.Empty,
                LocationName = sheet[r, 12].Value?.Trim() ?? string.Empty,
                Notes = NullIfEmpty(sheet[r, 13].Value?.Trim()),
            };

            // Tüm önemli alanları boşsa atla (excel'in altında boş satırlar olabilir)
            if (string.IsNullOrWhiteSpace(row.EmployeeNumber) &&
                string.IsNullOrWhiteSpace(row.FirstName) &&
                string.IsNullOrWhiteSpace(row.LastName))
            {
                continue;
            }

            rows.Add(row);
        }

        return rows;
    }

    private static DateOnly? ReadDate(IRange cell)
    {
        if (cell == null) return null;
        if (cell.HasDateTime)
        {
            return DateOnly.FromDateTime(cell.DateTime);
        }
        var v = cell.Value?.Trim();
        if (string.IsNullOrWhiteSpace(v)) return null;
        if (DateOnly.TryParse(v, CultureInfo.GetCultureInfo("tr-TR"), out var d)) return d;
        if (DateOnly.TryParse(v, CultureInfo.InvariantCulture, out d)) return d;
        return null;
    }

    private static string? NullIfEmpty(string? s) => string.IsNullOrWhiteSpace(s) ? null : s;
}
