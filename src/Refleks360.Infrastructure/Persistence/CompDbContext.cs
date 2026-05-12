using Microsoft.EntityFrameworkCore;
using Refleks360.Infrastructure.Persistence.Entities;
using Refleks360.Infrastructure.Persistence.Seed;

namespace Refleks360.Infrastructure.Persistence;

/// <summary>
/// Refleks 360 ÜP ana <see cref="DbContext"/>'i.
/// İlk migration <c>InitialCreate</c> sadece vergi tablolarını kurar; çalışan,
/// senaryo, audit gibi geri kalan şema sonraki haftalarda eklenir.
/// </summary>
public sealed class CompDbContext(DbContextOptions<CompDbContext> options) : DbContext(options)
{
    public DbSet<TaxYearEntity> TaxYears => Set<TaxYearEntity>();
    public DbSet<IncomeTaxBracketEntity> IncomeTaxBrackets => Set<IncomeTaxBracketEntity>();
    public DbSet<MonthlyTaxPeriodEntity> MonthlyTaxPeriods => Set<MonthlyTaxPeriodEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaxYearEntity>(b =>
        {
            b.ToTable("TaxYears");
            b.HasKey(e => e.Year);
            b.Property(e => e.Year).ValueGeneratedNever();
            b.Property(e => e.SgkEmployeeRate).HasColumnType("decimal(8,6)");
            b.Property(e => e.UnemploymentEmployeeRate).HasColumnType("decimal(8,6)");
            b.Property(e => e.SgkEmployerRate).HasColumnType("decimal(8,6)");
            b.Property(e => e.UnemploymentEmployerRate).HasColumnType("decimal(8,6)");
            b.Property(e => e.SgkEmployerDiscountRate).HasColumnType("decimal(8,6)");
            b.Property(e => e.StampTaxRate).HasColumnType("decimal(8,6)");
        });

        modelBuilder.Entity<IncomeTaxBracketEntity>(b =>
        {
            b.ToTable("IncomeTaxBrackets");
            b.HasKey(e => e.Id);
            b.Property(e => e.UpperLimit).HasColumnType("decimal(18,2)");
            b.Property(e => e.Rate).HasColumnType("decimal(8,6)");
            b.HasOne(e => e.TaxYear)
                .WithMany(y => y.IncomeTaxBrackets)
                .HasForeignKey(e => e.TaxYearId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(e => new { e.TaxYearId, e.OrderIndex }).IsUnique();
        });

        modelBuilder.Entity<MonthlyTaxPeriodEntity>(b =>
        {
            b.ToTable("MonthlyTaxPeriods");
            b.HasKey(e => e.Id);
            b.Property(e => e.SgkBaseMin).HasColumnType("decimal(18,2)");
            b.Property(e => e.SgkBaseMax).HasColumnType("decimal(18,2)");
            b.Property(e => e.GvExemptionAmount).HasColumnType("decimal(18,2)");
            b.Property(e => e.GvExemptionRate).HasColumnType("decimal(8,6)");
            b.Property(e => e.StampExemptionAmount).HasColumnType("decimal(18,4)");
            b.HasOne(e => e.TaxYear)
                .WithMany(y => y.MonthlyPeriods)
                .HasForeignKey(e => e.TaxYearId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(e => new { e.TaxYearId, e.StartMonth }).IsUnique();
        });

        TaxParameters2026Seed.Apply(modelBuilder);
    }
}
