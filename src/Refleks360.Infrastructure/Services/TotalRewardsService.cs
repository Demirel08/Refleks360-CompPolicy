using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using Refleks360.Application.Abstractions;
using Refleks360.Application.TotalRewards;
using Refleks360.Domain.Calculations;
using Refleks360.Domain.Organization;
using Refleks360.Infrastructure.Persistence;
using Refleks360.Infrastructure.Persistence.Entities;

namespace Refleks360.Infrastructure.Services;

public sealed class TotalRewardsService(CompDbContext db, ITaxParameterService taxParams) : ITotalRewardsService
{
    public async Task<TotalRewardsSummary?> GetForEmployeeAsync(int employeeId, CancellationToken ct = default)
    {
        var e = await db.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.Id == employeeId, ct);
        if (e is null) return null;
        var gross = await db.EmployeeSalaries.AsNoTracking()
            .Where(s => s.EmployeeId == employeeId && s.EndDate == null)
            .Select(s => (decimal?)s.GrossMonthly).FirstOrDefaultAsync(ct) ?? 0m;

        var benefits = await db.EmployeeBenefits.AsNoTracking()
            .Where(b => b.EmployeeId == employeeId && b.EndDate == null)
            .Select(b => new BenefitDto(b.Id, b.EmployeeId, b.BenefitType, b.MonthlyValue, b.Description, b.EffectiveDate, b.EndDate))
            .ToListAsync(ct);

        var benefitsTotal = benefits.Sum(b => b.MonthlyValue);

        // Yıllık ortalama işveren maliyeti
        var year = DateTime.UtcNow.Year;
        var tax = await taxParams.GetForYearAsync(year, ct);
        decimal cum = 0m, monthlyCost = 0m;
        for (int m = 1; m <= 12; m++)
        {
            var p = tax.PeriodsByMonth[m];
            var r = SalaryCalculator.CalculateMonth(gross, cum, p, tax.Parameters);
            monthlyCost += r.EmployerCost;
            cum += r.MonthlyTaxBase;
        }
        var annualEmployer = monthlyCost; // 12 ay toplamı zaten
        var annualGross = gross * 12m;
        var annualBenefits = benefitsTotal * 12m;
        var annualTotalComp = annualGross + annualBenefits + (annualEmployer - annualGross); // brüt + yan haklar + (işveren primleri)

        return new TotalRewardsSummary(
            e.Id, e.FirstName + " " + e.LastName,
            gross, annualGross,
            benefitsTotal,
            Math.Round(annualEmployer, 0),
            Math.Round(annualTotalComp, 0),
            benefits);
    }

    public async Task<int> AddBenefitAsync(int employeeId, BenefitType type, decimal monthlyValue, string? description, DateOnly effectiveDate, DateOnly? endDate, CancellationToken ct = default)
    {
        var b = new EmployeeBenefitEntity
        {
            EmployeeId = employeeId,
            BenefitType = type,
            MonthlyValue = monthlyValue,
            Description = description,
            EffectiveDate = effectiveDate,
            EndDate = endDate,
        };
        db.EmployeeBenefits.Add(b);
        await db.SaveChangesAsync(ct);
        return b.Id;
    }

    public async Task DeleteBenefitAsync(int benefitId, CancellationToken ct = default)
    {
        var b = await db.EmployeeBenefits.FirstAsync(x => x.Id == benefitId, ct);
        db.EmployeeBenefits.Remove(b);
        await db.SaveChangesAsync(ct);
    }

    public async Task<byte[]> GenerateCompLetterPdfAsync(int employeeId, decimal oldGross, decimal newGross, DateOnly effectiveDate, string reason, string signedByUserName, CancellationToken ct = default)
    {
        var e = await db.Employees.AsNoTracking()
            .Include(x => x.Department)
            .Include(x => x.Position)
            .FirstAsync(x => x.Id == employeeId, ct);
        var raisePct = oldGross > 0 ? Math.Round((newGross / oldGross - 1m) * 100m, 2) : 0m;

        var pdf = Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.Header().Text("Ücret Bildirim Mektubu").FontSize(20).Bold();
                page.Content().PaddingTop(20).Column(c =>
                {
                    c.Item().Text($"Sayın {e.FirstName} {e.LastName} ({e.EmployeeNumber}),").FontSize(11);
                    c.Item().PaddingTop(15).Text(
                        $"Şirketimizin ücret politikası kapsamında, {e.Position.Title} pozisyonundaki ücretinizde " +
                        $"{effectiveDate:dd.MM.yyyy} tarihinden geçerli olmak üzere aşağıdaki düzenleme yapılmıştır.")
                        .FontSize(11);
                    c.Item().PaddingTop(15).Border(1).Padding(10).Column(b =>
                    {
                        b.Item().Text($"Eski aylık brüt:  {oldGross:N2} TL").FontSize(11);
                        b.Item().Text($"Yeni aylık brüt:  {newGross:N2} TL").FontSize(11).Bold();
                        b.Item().Text($"Artış oranı:      %{raisePct:F2}").FontSize(11);
                        b.Item().Text($"Geçerlilik:       {effectiveDate:dd.MM.yyyy}").FontSize(11);
                        b.Item().Text($"Gerekçe:          {reason}").FontSize(11);
                    });
                    c.Item().PaddingTop(20).Text(
                        "Şirketimize yaptığınız değerli katkılar için teşekkür eder, çalışma hayatınızda başarılar dileriz.")
                        .FontSize(11);
                    c.Item().PaddingTop(40).AlignRight().Text($"{signedByUserName}").Bold();
                    c.Item().AlignRight().Text("İnsan Kaynakları").FontSize(10).Italic();
                });
                page.Footer().AlignCenter().Text($"Refleks 360 ÜP — {DateTime.Now:dd.MM.yyyy}").FontSize(8);
            });
        }).GeneratePdf();

        // DB'ye kaydet
        db.CompensationLetters.Add(new CompensationLetterEntity
        {
            EmployeeId = employeeId,
            EffectiveDate = effectiveDate,
            OldGross = oldGross,
            NewGross = newGross,
            RaisePercent = raisePct,
            Reason = reason,
            SignedByUserName = signedByUserName,
            PdfBlob = pdf,
        });
        await db.SaveChangesAsync(ct);

        return pdf;
    }
}
