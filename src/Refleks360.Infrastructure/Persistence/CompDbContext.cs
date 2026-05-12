using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Refleks360.Infrastructure.Identity;
using Refleks360.Infrastructure.Persistence.Entities;
using Refleks360.Infrastructure.Persistence.Seed;

namespace Refleks360.Infrastructure.Persistence;

/// <summary>
/// Refleks 360 ÜP ana <see cref="DbContext"/>'i. ASP.NET Identity tablolarını da
/// barındırır (<see cref="ApplicationUser"/> tabanlı). Vergi tabloları ve audit log
/// burada tek bir migration zinciri olarak yönetilir.
/// </summary>
public sealed class CompDbContext(DbContextOptions<CompDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<TaxYearEntity> TaxYears => Set<TaxYearEntity>();
    public DbSet<IncomeTaxBracketEntity> IncomeTaxBrackets => Set<IncomeTaxBracketEntity>();
    public DbSet<MonthlyTaxPeriodEntity> MonthlyTaxPeriods => Set<MonthlyTaxPeriodEntity>();
    public DbSet<AuditLogEntity> AuditLogs => Set<AuditLogEntity>();

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

        modelBuilder.Entity<AuditLogEntity>(b =>
        {
            b.ToTable("AuditLogs");
            b.HasKey(e => e.Id);
            b.Property(e => e.UserName).HasMaxLength(256).IsRequired();
            b.Property(e => e.Action).HasMaxLength(16).IsRequired();
            b.Property(e => e.EntityType).HasMaxLength(128).IsRequired();
            b.Property(e => e.EntityKey).HasMaxLength(256);
            b.HasIndex(e => e.TimestampUtc);
            b.HasIndex(e => new { e.EntityType, e.EntityKey });
        });

        modelBuilder.Entity<ApplicationUser>(b =>
        {
            b.Property(u => u.FullName).HasMaxLength(256).IsRequired();
        });

        TaxParameters2026Seed.Apply(modelBuilder);
    }
}
