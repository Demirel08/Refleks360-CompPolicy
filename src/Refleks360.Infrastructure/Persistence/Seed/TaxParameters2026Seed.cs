using Microsoft.EntityFrameworkCore;
using Refleks360.Infrastructure.Persistence.Entities;

namespace Refleks360.Infrastructure.Persistence.Seed;

/// <summary>
/// 2026 takvim yılı için <c>docs/05-HESAPLAMA-MOTORU.md</c>'deki resmi parametreler.
/// EF Core <see cref="ModelBuilder"/>.HasData() üzerinden migration'a gömülür.
/// </summary>
internal static class TaxParameters2026Seed
{
    /// <summary>
    /// Son dilim için "sonsuz" anlamına gelen sentinel. <c>decimal(18,2)</c> kolona
    /// sığmayan <see cref="decimal.MaxValue"/> yerine kullanılır. Servis katmanı bu
    /// değeri <see cref="decimal.MaxValue"/>'a çevirir.
    /// </summary>
    public const decimal InfinitySentinel = 9_999_999_999_999.99m;

    public const int Year2026 = 2026;
    public const decimal MinWageGross2026 = 33_030.00m;
    public const decimal SgkBaseMin2026 = MinWageGross2026;
    public const decimal SgkBaseMax2026 = MinWageGross2026 * 7.5m;     // 247_725.00
    public const decimal GvExemptionBase2026 = 28_075.50m;             // brut - işçi kesintileri
    public const decimal StampExemptionPerMonth2026 = 250.6977m;       // 33.030 × 0,00759

    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaxYearEntity>().HasData(new TaxYearEntity
        {
            Year = Year2026,
            SgkEmployeeRate = 0.14m,
            UnemploymentEmployeeRate = 0.01m,
            // 2025 sonrası: GSS işveren payı %7,5 -> %8,5 oldu. Toplam SGK işveren primi
            // %20,75'ten %21,75'e çıktı. Verginet.net ve diğer güncel hesaplayıcılar bu oranı kullanır.
            SgkEmployerRate = 0.2175m,
            UnemploymentEmployerRate = 0.02m,
            SgkEmployerDiscountRate = 0.05m,
            ApplySgkEmployerDiscount = false,
            StampTaxRate = 0.00759m,
        });

        // Gelir vergisi dilimleri 2026 (ücret tarifesi — GVK md.103)
        modelBuilder.Entity<IncomeTaxBracketEntity>().HasData(
            new IncomeTaxBracketEntity { Id = 1, TaxYearId = Year2026, OrderIndex = 0, UpperLimit =     190_000m, Rate = 0.15m },
            new IncomeTaxBracketEntity { Id = 2, TaxYearId = Year2026, OrderIndex = 1, UpperLimit =     400_000m, Rate = 0.20m },
            new IncomeTaxBracketEntity { Id = 3, TaxYearId = Year2026, OrderIndex = 2, UpperLimit =   1_500_000m, Rate = 0.27m },
            new IncomeTaxBracketEntity { Id = 4, TaxYearId = Year2026, OrderIndex = 3, UpperLimit =   5_300_000m, Rate = 0.35m },
            new IncomeTaxBracketEntity { Id = 5, TaxYearId = Year2026, OrderIndex = 4, UpperLimit = InfinitySentinel, Rate = 0.40m });

        // 12 aylık dönemler. Spec: 1-7. aylar GV istisna oranı %15, 8-12. aylar %20.
        var monthSeed = new List<MonthlyTaxPeriodEntity>(12);
        int periodId = 1;
        for (int month = 1; month <= 12; month++)
        {
            monthSeed.Add(new MonthlyTaxPeriodEntity
            {
                Id = periodId++,
                TaxYearId = Year2026,
                StartMonth = month,
                EndMonth = month,
                SgkBaseMin = SgkBaseMin2026,
                SgkBaseMax = SgkBaseMax2026,
                GvExemptionAmount = GvExemptionBase2026,
                GvExemptionRate = month <= 7 ? 0.15m : 0.20m,
                StampExemptionAmount = StampExemptionPerMonth2026,
            });
        }

        modelBuilder.Entity<MonthlyTaxPeriodEntity>().HasData(monthSeed);
    }
}
