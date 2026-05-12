using Microsoft.EntityFrameworkCore;
using Refleks360.Domain.Organization;
using Refleks360.Infrastructure.Persistence.Entities;

namespace Refleks360.Infrastructure.Persistence.Seed;

internal static class SalarySeed
{
    private static readonly DateOnly Effective = new(2026, 1, 1);

    public static void Apply(ModelBuilder modelBuilder)
    {
        // 6 kademe için makul bantlar (TL aylık brüt). Lokasyon bağımsız.
        modelBuilder.Entity<SalaryBandEntity>().HasData(
            new SalaryBandEntity { Id = 1, JobGradeId = 1, LocationId = null, Min =  40_000m, Mid =  50_000m, Max =  60_000m, EffectiveDate = Effective },
            new SalaryBandEntity { Id = 2, JobGradeId = 2, LocationId = null, Min =  55_000m, Mid =  70_000m, Max =  85_000m, EffectiveDate = Effective },
            new SalaryBandEntity { Id = 3, JobGradeId = 3, LocationId = null, Min =  80_000m, Mid = 100_000m, Max = 120_000m, EffectiveDate = Effective },
            new SalaryBandEntity { Id = 4, JobGradeId = 4, LocationId = null, Min = 110_000m, Mid = 140_000m, Max = 170_000m, EffectiveDate = Effective },
            new SalaryBandEntity { Id = 5, JobGradeId = 5, LocationId = null, Min = 160_000m, Mid = 200_000m, Max = 240_000m, EffectiveDate = Effective },
            new SalaryBandEntity { Id = 6, JobGradeId = 6, LocationId = null, Min = 240_000m, Mid = 300_000m, Max = 360_000m, EffectiveDate = Effective }
        );

        // 30 çalışan için başlangıç ücreti (mid civarı ±%15 dalgalanma). Kademe pos.JobGradeId üzerinden.
        // Pos -> Grade mapping (OrganizationSeed'den):
        //   pos 1->1, 2->2, 3->3, 4->4, 5->2, 6->5, 7->3, 8->2, 9->1, 10->3
        var posToGrade = new Dictionary<int, int>
        {
            { 1, 1 }, { 2, 2 }, { 3, 3 }, { 4, 4 }, { 5, 2 },
            { 6, 5 }, { 7, 3 }, { 8, 2 }, { 9, 1 }, { 10, 3 },
        };
        // OrganizationSeed'deki pos[] array: 30 elemanlı, çalışan id'ye göre pozisyon.
        var pos = new int[] { 2,2,2,3,3,3,1,1,1,4,4,4, 5,5,5,6, 7,7,7,3, 8,8,8,8,8, 9,9,9,9,10 };

        var gradeBandMid = new Dictionary<int, decimal>
        {
            { 1,  50_000m }, { 2,  70_000m }, { 3, 100_000m },
            { 4, 140_000m }, { 5, 200_000m }, { 6, 300_000m },
        };
        // Sabit (deterministik) varyasyon: i'ye göre -%15..+%15 arası faktör
        var factors = new decimal[] { 0.85m, 0.92m, 1.00m, 1.05m, 0.98m, 1.10m, 0.88m, 1.03m, 0.95m, 1.12m,
                                      0.90m, 1.05m, 0.96m, 1.02m, 0.99m, 1.08m, 0.93m, 1.06m, 1.00m, 0.97m,
                                      0.91m, 1.04m, 1.00m, 0.94m, 1.07m, 0.89m, 1.01m, 0.98m, 1.05m, 0.96m };

        var seedDate = new DateOnly(2026, 1, 1);
        var salaries = new List<EmployeeSalaryEntity>(30);
        for (int i = 0; i < 30; i++)
        {
            var posId = pos[i];
            var gradeId = posToGrade[posId];
            var mid = gradeBandMid[gradeId];
            var gross = Math.Round(mid * factors[i], 0);
            salaries.Add(new EmployeeSalaryEntity
            {
                Id = i + 1,
                EmployeeId = i + 1,
                GrossMonthly = gross,
                EffectiveDate = seedDate,
                Reason = SalaryChangeReason.NewHire,
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            });
        }
        modelBuilder.Entity<EmployeeSalaryEntity>().HasData(salaries);
    }
}
