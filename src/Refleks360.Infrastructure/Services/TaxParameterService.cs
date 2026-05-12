using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Refleks360.Application.Abstractions;
using Refleks360.Application.Calculations;
using Refleks360.Domain.Calculations;
using Refleks360.Infrastructure.Persistence;
using Refleks360.Infrastructure.Persistence.Seed;

namespace Refleks360.Infrastructure.Services;

/// <summary>
/// <see cref="ITaxParameterService"/>'in EF Core + bellek-içi cache uygulaması.
/// Bir kez okunan yıl <see cref="CacheTtl"/> süresince RAM'de tutulur — vergi
/// parametreleri yılda 1-2 defa değiştiği için bu hem doğru hem ucuz.
/// </summary>
public sealed class TaxParameterService(CompDbContext db, IMemoryCache cache) : ITaxParameterService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(12);

    public async Task<YearTaxData> GetForYearAsync(int year, CancellationToken ct = default)
    {
        var cacheKey = $"tax-year:{year}";

        if (cache.TryGetValue<YearTaxData>(cacheKey, out var cached) && cached is not null)
        {
            return cached;
        }

        var taxYear = await db.TaxYears
            .AsNoTracking()
            .Include(y => y.IncomeTaxBrackets)
            .Include(y => y.MonthlyPeriods)
            .FirstOrDefaultAsync(y => y.Year == year, ct)
            ?? throw new InvalidOperationException(
                $"{year} yılı için vergi parametreleri tabloda bulunamadı. " +
                "Migration ve seed'in uygulandığından emin ol (dotnet ef database update).");

        var brackets = taxYear.IncomeTaxBrackets
            .OrderBy(b => b.OrderIndex)
            .Select(b => new TaxBracket(
                UpperLimit: b.UpperLimit >= TaxParameters2026Seed.InfinitySentinel
                    ? decimal.MaxValue
                    : b.UpperLimit,
                Rate: b.Rate))
            .ToList();

        var parameters = new TaxParameters
        {
            SgkEmployeeRate = taxYear.SgkEmployeeRate,
            UnemploymentEmployeeRate = taxYear.UnemploymentEmployeeRate,
            SgkEmployerRate = taxYear.SgkEmployerRate,
            UnemploymentEmployerRate = taxYear.UnemploymentEmployerRate,
            SgkEmployerDiscountRate = taxYear.SgkEmployerDiscountRate,
            ApplySgkEmployerDiscount = taxYear.ApplySgkEmployerDiscount,
            StampTaxRate = taxYear.StampTaxRate,
            IncomeTaxBrackets = brackets,
        };

        var periodsByMonth = new Dictionary<int, MonthlyTaxPeriod>(12);
        foreach (var p in taxYear.MonthlyPeriods.OrderBy(p => p.StartMonth))
        {
            var period = new MonthlyTaxPeriod(
                StartMonth: p.StartMonth,
                EndMonth: p.EndMonth,
                SgkBaseMin: p.SgkBaseMin,
                SgkBaseMax: p.SgkBaseMax,
                GvExemptionAmount: p.GvExemptionAmount,
                GvExemptionRate: p.GvExemptionRate,
                StampExemptionAmount: p.StampExemptionAmount);

            for (int m = p.StartMonth; m <= p.EndMonth; m++)
            {
                periodsByMonth[m] = period;
            }
        }

        var data = new YearTaxData(year, parameters, periodsByMonth);
        cache.Set(cacheKey, data, CacheTtl);
        return data;
    }
}
