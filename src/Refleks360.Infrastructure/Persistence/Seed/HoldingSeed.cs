using Microsoft.EntityFrameworkCore;
using Refleks360.Infrastructure.Persistence.Entities;

namespace Refleks360.Infrastructure.Persistence.Seed;

/// <summary>
/// İkinci bir demo şirket — holding yapısını sergilemek için. Mevcut çalışan/lokasyon/
/// departman verileri 1. şirkete bağlıdır; bu şirket ileride yeni veri girilerek kullanılır.
/// </summary>
internal static class HoldingSeed
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CompanyEntity>().HasData(new CompanyEntity
        {
            Id = 2,
            Name = "Refleks Demo Yatırım A.Ş.",
            LegalName = "Refleks Demo Yatırım Holding Anonim Şirketi",
            TaxNo = "0000000001",
            FiscalYearStartMonth = 1,
            Currency = "TL",
            IsActive = true,
        });
    }
}
