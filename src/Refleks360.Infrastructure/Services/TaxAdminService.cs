using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Abstractions;
using Refleks360.Application.TaxAdmin;
using Refleks360.Infrastructure.Persistence;
using Refleks360.Infrastructure.Persistence.Entities;

namespace Refleks360.Infrastructure.Services;

public sealed class TaxAdminService(CompDbContext db) : ITaxAdminService
{
    public async Task<IReadOnlyList<int>> GetYearsAsync(CancellationToken ct = default) =>
        await db.TaxYears.AsNoTracking().OrderByDescending(y => y.Year).Select(y => y.Year).ToListAsync(ct);

    public async Task<TaxYearDetail?> GetAsync(int year, CancellationToken ct = default)
    {
        var y = await db.TaxYears.AsNoTracking()
            .Include(t => t.IncomeTaxBrackets)
            .Include(t => t.MonthlyPeriods)
            .FirstOrDefaultAsync(t => t.Year == year, ct);
        if (y is null) return null;

        return new TaxYearDetail(
            new TaxYearAdminDto(y.Year, y.SgkEmployeeRate, y.UnemploymentEmployeeRate, y.SgkEmployerRate, y.UnemploymentEmployerRate, y.SgkEmployerDiscountRate, y.ApplySgkEmployerDiscount, y.StampTaxRate),
            y.IncomeTaxBrackets.OrderBy(b => b.OrderIndex)
                .Select(b => new TaxBracketAdminDto(b.Id, b.OrderIndex, b.UpperLimit, b.Rate)).ToList(),
            y.MonthlyPeriods.OrderBy(p => p.StartMonth)
                .Select(p => new MonthlyPeriodAdminDto(p.Id, p.StartMonth, p.EndMonth, p.SgkBaseMin, p.SgkBaseMax, p.GvExemptionAmount, p.GvExemptionRate, p.StampExemptionAmount)).ToList());
    }

    public async Task SaveYearAsync(TaxYearAdminDto y, CancellationToken ct = default)
    {
        ValidateRates(y);

        var entity = await db.TaxYears.FirstOrDefaultAsync(t => t.Year == y.Year, ct);
        if (entity is null)
        {
            entity = new TaxYearEntity { Year = y.Year };
            db.TaxYears.Add(entity);
        }
        entity.SgkEmployeeRate = y.SgkEmployeeRate;
        entity.UnemploymentEmployeeRate = y.UnemploymentEmployeeRate;
        entity.SgkEmployerRate = y.SgkEmployerRate;
        entity.UnemploymentEmployerRate = y.UnemploymentEmployerRate;
        entity.SgkEmployerDiscountRate = y.SgkEmployerDiscountRate;
        entity.ApplySgkEmployerDiscount = y.ApplySgkEmployerDiscount;
        entity.StampTaxRate = y.StampTaxRate;
        await db.SaveChangesAsync(ct);
    }

    private static void ValidateRates(TaxYearAdminDto y)
    {
        // Yanlış birim hatalarını engellemek için makul sınır kontrolü
        Check(y.SgkEmployeeRate, 0.08m, 0.20m, "SGK İşçi", "~%14");
        Check(y.UnemploymentEmployeeRate, 0.005m, 0.05m, "İşsizlik İşçi", "~%1");
        Check(y.SgkEmployerRate, 0.15m, 0.30m, "SGK İşveren", "~%21,75");
        Check(y.UnemploymentEmployerRate, 0.005m, 0.05m, "İşsizlik İşveren", "~%2");
        Check(y.SgkEmployerDiscountRate, 0m, 0.15m, "SGK İşveren İndirim", "~%5");
        Check(y.StampTaxRate, 0.003m, 0.015m, "Damga", "~%0,759");

        static void Check(decimal value, decimal min, decimal max, string name, string normal)
        {
            if (value < min || value > max)
                throw new InvalidOperationException(
                    $"'{name}' oranı geçersiz: {value:P2}. Beklenen: {min:P2} - {max:P2} (normalde {normal}). " +
                    $"Lütfen değeri PERCENT olarak gir (örn. 21,75 = %21,75).");
        }
    }

    public async Task SaveBracketsAsync(int year, IReadOnlyList<TaxBracketAdminDto> brackets, CancellationToken ct = default)
    {
        var existing = await db.IncomeTaxBrackets.Where(b => b.TaxYearId == year).ToListAsync(ct);
        db.IncomeTaxBrackets.RemoveRange(existing);
        int i = 0;
        foreach (var b in brackets.OrderBy(x => x.OrderIndex))
        {
            db.IncomeTaxBrackets.Add(new IncomeTaxBracketEntity
            {
                TaxYearId = year, OrderIndex = i++, UpperLimit = b.UpperLimit, Rate = b.Rate,
            });
        }
        await db.SaveChangesAsync(ct);
    }

    public async Task SavePeriodsAsync(int year, IReadOnlyList<MonthlyPeriodAdminDto> periods, CancellationToken ct = default)
    {
        var existing = await db.MonthlyTaxPeriods.Where(p => p.TaxYearId == year).ToListAsync(ct);
        db.MonthlyTaxPeriods.RemoveRange(existing);
        foreach (var p in periods.OrderBy(x => x.StartMonth))
        {
            db.MonthlyTaxPeriods.Add(new MonthlyTaxPeriodEntity
            {
                TaxYearId = year, StartMonth = p.StartMonth, EndMonth = p.EndMonth,
                SgkBaseMin = p.SgkBaseMin, SgkBaseMax = p.SgkBaseMax,
                GvExemptionAmount = p.GvExemptionAmount, GvExemptionRate = p.GvExemptionRate,
                StampExemptionAmount = p.StampExemptionAmount,
            });
        }
        await db.SaveChangesAsync(ct);
    }

    public async Task<int> CloneFromAsync(int sourceYear, int targetYear, CancellationToken ct = default)
    {
        if (await db.TaxYears.AnyAsync(t => t.Year == targetYear, ct))
            throw new InvalidOperationException($"{targetYear} yılı zaten var.");

        var src = await db.TaxYears
            .Include(t => t.IncomeTaxBrackets)
            .Include(t => t.MonthlyPeriods)
            .FirstAsync(t => t.Year == sourceYear, ct);

        db.TaxYears.Add(new TaxYearEntity
        {
            Year = targetYear,
            SgkEmployeeRate = src.SgkEmployeeRate,
            UnemploymentEmployeeRate = src.UnemploymentEmployeeRate,
            SgkEmployerRate = src.SgkEmployerRate,
            UnemploymentEmployerRate = src.UnemploymentEmployerRate,
            SgkEmployerDiscountRate = src.SgkEmployerDiscountRate,
            ApplySgkEmployerDiscount = src.ApplySgkEmployerDiscount,
            StampTaxRate = src.StampTaxRate,
        });

        int i = 0;
        foreach (var b in src.IncomeTaxBrackets.OrderBy(x => x.OrderIndex))
        {
            db.IncomeTaxBrackets.Add(new IncomeTaxBracketEntity
            {
                TaxYearId = targetYear, OrderIndex = i++, UpperLimit = b.UpperLimit, Rate = b.Rate,
            });
        }
        foreach (var p in src.MonthlyPeriods.OrderBy(x => x.StartMonth))
        {
            db.MonthlyTaxPeriods.Add(new MonthlyTaxPeriodEntity
            {
                TaxYearId = targetYear, StartMonth = p.StartMonth, EndMonth = p.EndMonth,
                SgkBaseMin = p.SgkBaseMin, SgkBaseMax = p.SgkBaseMax,
                GvExemptionAmount = p.GvExemptionAmount, GvExemptionRate = p.GvExemptionRate,
                StampExemptionAmount = p.StampExemptionAmount,
            });
        }

        await db.SaveChangesAsync(ct);
        return targetYear;
    }
}
