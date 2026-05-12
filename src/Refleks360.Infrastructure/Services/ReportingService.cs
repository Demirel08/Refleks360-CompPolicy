using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Refleks360.Application.Abstractions;
using Refleks360.Domain.Calculations;
using Refleks360.Domain.Scenarios;
using Refleks360.Infrastructure.Persistence;
using Syncfusion.XlsIO;

namespace Refleks360.Infrastructure.Services;

public sealed class ReportingService(
    CompDbContext db,
    ITaxParameterService taxParams,
    ICompPolicyMetricsService compMetrics) : IReportingService
{
    static ReportingService()
    {
        // QuestPDF community license (ücretsiz, $1M altı işletmeler için).
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> ExportEmployeeListXlsxAsync(CancellationToken ct = default)
    {
        var rows = await db.Employees.AsNoTracking()
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .Select(e => new
            {
                e.EmployeeNumber,
                FullName = e.FirstName + " " + e.LastName,
                Position = e.Position.Title,
                Grade = e.Position.JobGrade.Code,
                Department = e.Department.Name,
                Location = e.Location.Name,
                e.Status,
                e.Gender,
                e.HireDate,
                Gross = db.EmployeeSalaries.Where(s => s.EmployeeId == e.Id && s.EndDate == null)
                    .Select(s => (decimal?)s.GrossMonthly).FirstOrDefault(),
            })
            .ToListAsync(ct);

        using var engine = new ExcelEngine();
        engine.Excel.DefaultVersion = ExcelVersion.Xlsx;
        var book = engine.Excel.Workbooks.Create(1);
        var sheet = book.Worksheets[0];
        sheet.Name = "Calisanlar";

        string[] headers = { "Sicil", "Ad Soyad", "Pozisyon", "Kademe", "Departman", "Lokasyon", "Durum", "Cinsiyet", "İşe Giriş", "Aylık Brüt" };
        for (int i = 0; i < headers.Length; i++) sheet.SetText(1, i + 1, headers[i]);
        sheet.Range[1, 1, 1, headers.Length].CellStyle.Font.Bold = true;

        int r = 2;
        foreach (var row in rows)
        {
            sheet.SetText(r, 1, row.EmployeeNumber);
            sheet.SetText(r, 2, row.FullName);
            sheet.SetText(r, 3, row.Position);
            sheet.SetText(r, 4, row.Grade);
            sheet.SetText(r, 5, row.Department);
            sheet.SetText(r, 6, row.Location);
            sheet.SetText(r, 7, row.Status.ToString());
            sheet.SetText(r, 8, row.Gender.ToString());
            sheet[r, 9].DateTime = row.HireDate.ToDateTime(TimeOnly.MinValue);
            sheet[r, 9].NumberFormat = "dd.MM.yyyy";
            if (row.Gross.HasValue) sheet[r, 10].Number = (double)row.Gross.Value;
            r++;
        }

        sheet.UsedRange.AutofitColumns();
        using var ms = new MemoryStream();
        book.SaveAs(ms);
        return ms.ToArray();
    }

    public async Task<byte[]> ExportCompPolicyXlsxAsync(CancellationToken ct = default)
    {
        var data = await compMetrics.ComputeOverviewAsync(ct);
        using var engine = new ExcelEngine();
        engine.Excel.DefaultVersion = ExcelVersion.Xlsx;
        var book = engine.Excel.Workbooks.Create(2);
        var s1 = book.Worksheets[0];
        s1.Name = "Histogram";
        s1.SetText(1, 1, "Aralık"); s1.SetText(1, 2, "Çalışan");
        s1.Range[1, 1, 1, 2].CellStyle.Font.Bold = true;
        int rr = 2;
        foreach (var b in data.Histogram) { s1.SetText(rr, 1, b.Label); s1.SetNumber(rr, 2, b.Count); rr++; }
        s1.UsedRange.AutofitColumns();

        var s2 = book.Worksheets[1];
        s2.Name = "BantDisi";
        s2.SetText(1, 1, "Sicil"); s2.SetText(1, 2, "Ad Soyad"); s2.SetText(1, 3, "Departman");
        s2.SetText(1, 4, "Brüt"); s2.SetText(1, 5, "Min"); s2.SetText(1, 6, "Max"); s2.SetText(1, 7, "Durum");
        s2.Range[1, 1, 1, 7].CellStyle.Font.Bold = true;
        rr = 2;
        foreach (var o in data.OutOfBand)
        {
            s2.SetText(rr, 1, o.EmployeeNumber);
            s2.SetText(rr, 2, o.FullName);
            s2.SetText(rr, 3, o.Department);
            s2.SetNumber(rr, 4, (double)o.CurrentGross);
            s2.SetNumber(rr, 5, (double)o.BandMin);
            s2.SetNumber(rr, 6, (double)o.BandMax);
            s2.SetText(rr, 7, o.Flag);
            rr++;
        }
        s2.UsedRange.AutofitColumns();

        using var ms = new MemoryStream();
        book.SaveAs(ms);
        return ms.ToArray();
    }

    public async Task<byte[]> ExportScenarioPdfAsync(int scenarioId, CancellationToken ct = default)
    {
        var sc = await db.Scenarios.AsNoTracking().FirstAsync(s => s.Id == scenarioId, ct);
        var rows = await db.ScenarioEmployees.AsNoTracking()
            .Where(e => e.ScenarioId == scenarioId)
            .Select(e => new
            {
                e.Employee.EmployeeNumber,
                FullName = e.Employee.FirstName + " " + e.Employee.LastName,
                Department = e.Employee.Department.Name,
                e.OldGross, e.NewGross, e.RaisePercent, e.RaiseAmount,
                e.OldEmployerCost, e.NewEmployerCost,
            })
            .OrderBy(e => e.FullName)
            .ToListAsync(ct);

        return Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4.Landscape());

                page.Header().Column(c =>
                {
                    c.Item().Text($"Senaryo Özeti: {sc.Name}").FontSize(18).Bold();
                    c.Item().Text($"Tip: {sc.Type}    Durum: {sc.Status}    Baz tarih: {sc.BaseDate:dd.MM.yyyy}").FontSize(10);
                    if (!string.IsNullOrWhiteSpace(sc.Description))
                        c.Item().Text(sc.Description).FontSize(9).Italic();
                });

                page.Content().PaddingTop(10).Table(t =>
                {
                    t.ColumnsDefinition(cd =>
                    {
                        cd.RelativeColumn(1);  // sicil
                        cd.RelativeColumn(2.5f); // ad soyad
                        cd.RelativeColumn(2);  // dept
                        cd.RelativeColumn(1.2f); // eski brut
                        cd.RelativeColumn(1.2f); // yeni brut
                        cd.RelativeColumn(0.8f); // %
                        cd.RelativeColumn(1.2f); // raise tl
                        cd.RelativeColumn(1.4f); // old emp cost
                        cd.RelativeColumn(1.4f); // new emp cost
                    });

                    t.Header(h =>
                    {
                        h.Cell().Text("Sicil").Bold();
                        h.Cell().Text("Ad Soyad").Bold();
                        h.Cell().Text("Departman").Bold();
                        h.Cell().AlignRight().Text("Eski Brüt").Bold();
                        h.Cell().AlignRight().Text("Yeni Brüt").Bold();
                        h.Cell().AlignRight().Text("%").Bold();
                        h.Cell().AlignRight().Text("+TL").Bold();
                        h.Cell().AlignRight().Text("Eski İşv.").Bold();
                        h.Cell().AlignRight().Text("Yeni İşv.").Bold();
                    });

                    foreach (var r in rows)
                    {
                        t.Cell().Text(r.EmployeeNumber).FontSize(9);
                        t.Cell().Text(r.FullName).FontSize(9);
                        t.Cell().Text(r.Department).FontSize(9);
                        t.Cell().AlignRight().Text(r.OldGross.ToString("N0")).FontSize(9);
                        t.Cell().AlignRight().Text(r.NewGross.ToString("N0")).FontSize(9);
                        t.Cell().AlignRight().Text(r.RaisePercent.ToString("F1")).FontSize(9);
                        t.Cell().AlignRight().Text(r.RaiseAmount.ToString("N0")).FontSize(9);
                        t.Cell().AlignRight().Text(r.OldEmployerCost.ToString("N0")).FontSize(9);
                        t.Cell().AlignRight().Text(r.NewEmployerCost.ToString("N0")).FontSize(9);
                    }
                });

                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Refleks 360 ÜP — ");
                    t.Span(DateTime.Now.ToString("g"));
                    t.Span("   sayfa ");
                    t.CurrentPageNumber();
                    t.Span(" / ");
                    t.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    public async Task<byte[]> ExportMonthlyPayrollPdfAsync(int year, int month, CancellationToken ct = default)
    {
        var tax = await taxParams.GetForYearAsync(year, ct);
        var period = tax.PeriodsByMonth[month];

        var employees = await db.Employees.AsNoTracking()
            .Where(e => e.Status == Domain.Organization.EmployeeStatus.Active)
            .Select(e => new
            {
                e.EmployeeNumber,
                FullName = e.FirstName + " " + e.LastName,
                Department = e.Department.Name,
                Position = e.Position.Title,
                Gross = db.EmployeeSalaries
                    .Where(s => s.EmployeeId == e.Id && s.EndDate == null)
                    .Select(s => (decimal?)s.GrossMonthly).FirstOrDefault(),
            })
            .ToListAsync(ct);

        var rows = employees
            .Where(e => e.Gross.HasValue)
            .Select(e =>
            {
                var r = SalaryCalculator.CalculateMonth(e.Gross!.Value, 0m, period, tax.Parameters);
                return new { e.EmployeeNumber, e.FullName, e.Department, e.Position, Gross = e.Gross.Value, r.Net, r.EmployerCost };
            }).ToList();

        return Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4.Landscape());
                page.Header().Text($"Aylık Ücret Raporu — {month:00}/{year}").FontSize(18).Bold();
                page.Content().PaddingTop(10).Table(t =>
                {
                    t.ColumnsDefinition(cd =>
                    {
                        cd.RelativeColumn(1); cd.RelativeColumn(2.5f); cd.RelativeColumn(2); cd.RelativeColumn(2.5f);
                        cd.RelativeColumn(1.4f); cd.RelativeColumn(1.4f); cd.RelativeColumn(1.6f);
                    });
                    t.Header(h =>
                    {
                        h.Cell().Text("Sicil").Bold();
                        h.Cell().Text("Ad Soyad").Bold();
                        h.Cell().Text("Departman").Bold();
                        h.Cell().Text("Pozisyon").Bold();
                        h.Cell().AlignRight().Text("Brüt").Bold();
                        h.Cell().AlignRight().Text("Net").Bold();
                        h.Cell().AlignRight().Text("İşveren Mal.").Bold();
                    });
                    foreach (var r in rows)
                    {
                        t.Cell().Text(r.EmployeeNumber).FontSize(9);
                        t.Cell().Text(r.FullName).FontSize(9);
                        t.Cell().Text(r.Department).FontSize(9);
                        t.Cell().Text(r.Position).FontSize(9);
                        t.Cell().AlignRight().Text(r.Gross.ToString("N0")).FontSize(9);
                        t.Cell().AlignRight().Text(r.Net.ToString("N0")).FontSize(9);
                        t.Cell().AlignRight().Text(r.EmployerCost.ToString("N0")).FontSize(9);
                    }

                    // Toplam satırı
                    t.Cell().ColumnSpan(4).Text("TOPLAM").Bold();
                    t.Cell().AlignRight().Text(rows.Sum(r => r.Gross).ToString("N0")).Bold();
                    t.Cell().AlignRight().Text(rows.Sum(r => r.Net).ToString("N0")).Bold();
                    t.Cell().AlignRight().Text(rows.Sum(r => r.EmployerCost).ToString("N0")).Bold();
                });
                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Refleks 360 ÜP — ");
                    t.Span(DateTime.Now.ToString("g"));
                });
            });
        }).GeneratePdf();
    }
}
