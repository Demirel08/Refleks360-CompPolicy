using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Refleks360.Infrastructure.Identity;
using Refleks360.Infrastructure.Persistence.Entities;
using Refleks360.Infrastructure.Persistence.Seed;

namespace Refleks360.Infrastructure.Persistence;

/// <summary>
/// Refleks 360 ÜP ana <see cref="DbContext"/>'i. Identity, vergi, organizasyon
/// (şirket/lokasyon/departman/pozisyon/çalışan) ve audit log tablolarını barındırır.
/// </summary>
public sealed class CompDbContext(DbContextOptions<CompDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<TaxYearEntity> TaxYears => Set<TaxYearEntity>();
    public DbSet<IncomeTaxBracketEntity> IncomeTaxBrackets => Set<IncomeTaxBracketEntity>();
    public DbSet<MonthlyTaxPeriodEntity> MonthlyTaxPeriods => Set<MonthlyTaxPeriodEntity>();
    public DbSet<AuditLogEntity> AuditLogs => Set<AuditLogEntity>();

    public DbSet<CompanyEntity> Companies => Set<CompanyEntity>();
    public DbSet<LocationEntity> Locations => Set<LocationEntity>();
    public DbSet<DepartmentEntity> Departments => Set<DepartmentEntity>();
    public DbSet<JobFamilyEntity> JobFamilies => Set<JobFamilyEntity>();
    public DbSet<JobGradeEntity> JobGrades => Set<JobGradeEntity>();
    public DbSet<PositionEntity> Positions => Set<PositionEntity>();
    public DbSet<EmployeeEntity> Employees => Set<EmployeeEntity>();
    public DbSet<SalaryBandEntity> SalaryBands => Set<SalaryBandEntity>();
    public DbSet<EmployeeSalaryEntity> EmployeeSalaries => Set<EmployeeSalaryEntity>();
    public DbSet<ScenarioEntity> Scenarios => Set<ScenarioEntity>();
    public DbSet<ScenarioEmployeeEntity> ScenarioEmployees => Set<ScenarioEmployeeEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureTaxSchema(modelBuilder);
        ConfigureAuditSchema(modelBuilder);
        ConfigureOrganizationSchema(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(b =>
        {
            b.Property(u => u.FullName).HasMaxLength(256).IsRequired();
        });

        TaxParameters2026Seed.Apply(modelBuilder);
        OrganizationSeed.Apply(modelBuilder);
        SalarySeed.Apply(modelBuilder);
    }

    private static void ConfigureTaxSchema(ModelBuilder modelBuilder)
    {
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
    }

    private static void ConfigureAuditSchema(ModelBuilder modelBuilder)
    {
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
    }

    private static void ConfigureOrganizationSchema(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CompanyEntity>(b =>
        {
            b.ToTable("Companies");
            b.HasKey(e => e.Id);
            b.Property(e => e.Name).HasMaxLength(256).IsRequired();
            b.Property(e => e.LegalName).HasMaxLength(256).IsRequired();
            b.Property(e => e.TaxNo).HasMaxLength(32).IsRequired();
            b.Property(e => e.Currency).HasMaxLength(8).IsRequired();
            b.HasIndex(e => e.TaxNo).IsUnique();
        });

        modelBuilder.Entity<LocationEntity>(b =>
        {
            b.ToTable("Locations");
            b.HasKey(e => e.Id);
            b.Property(e => e.Name).HasMaxLength(128).IsRequired();
            b.Property(e => e.City).HasMaxLength(64).IsRequired();
            b.Property(e => e.Country).HasMaxLength(64).IsRequired();
            b.Property(e => e.RegionalIndexPercent).HasColumnType("decimal(6,2)");
            b.HasOne(e => e.Company)
                .WithMany(c => c.Locations)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DepartmentEntity>(b =>
        {
            b.ToTable("Departments");
            b.HasKey(e => e.Id);
            b.Property(e => e.Name).HasMaxLength(128).IsRequired();
            b.Property(e => e.CostCenterCode).HasMaxLength(32);
            b.HasOne(e => e.Company)
                .WithMany(c => c.Departments)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.ParentDepartment)
                .WithMany(d => d.ChildDepartments)
                .HasForeignKey(e => e.ParentDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.ManagerEmployee)
                .WithMany()
                .HasForeignKey(e => e.ManagerEmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
            b.HasIndex(e => new { e.CompanyId, e.Name });
        });

        modelBuilder.Entity<JobFamilyEntity>(b =>
        {
            b.ToTable("JobFamilies");
            b.HasKey(e => e.Id);
            b.Property(e => e.Name).HasMaxLength(128).IsRequired();
            b.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<JobGradeEntity>(b =>
        {
            b.ToTable("JobGrades");
            b.HasKey(e => e.Id);
            b.Property(e => e.Code).HasMaxLength(16).IsRequired();
            b.Property(e => e.Name).HasMaxLength(128).IsRequired();
            b.HasIndex(e => e.Code).IsUnique();
            b.HasIndex(e => e.OrderIndex);
        });

        modelBuilder.Entity<PositionEntity>(b =>
        {
            b.ToTable("Positions");
            b.HasKey(e => e.Id);
            b.Property(e => e.Title).HasMaxLength(256).IsRequired();
            b.Property(e => e.BenchmarkMatchName).HasMaxLength(256);
            b.HasOne(e => e.JobGrade)
                .WithMany()
                .HasForeignKey(e => e.JobGradeId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.JobFamily)
                .WithMany()
                .HasForeignKey(e => e.JobFamilyId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(e => e.Title);
        });

        modelBuilder.Entity<EmployeeEntity>(b =>
        {
            b.ToTable("Employees");
            b.HasKey(e => e.Id);
            b.Property(e => e.EmployeeNumber).HasMaxLength(32).IsRequired();
            b.Property(e => e.FirstName).HasMaxLength(128).IsRequired();
            b.Property(e => e.LastName).HasMaxLength(128).IsRequired();
            b.Property(e => e.Email).HasMaxLength(256);
            b.Property(e => e.Phone).HasMaxLength(32);
            b.HasIndex(e => e.EmployeeNumber).IsUnique();
            b.HasIndex(e => e.DepartmentId);
            b.HasIndex(e => e.ManagerId);
            b.HasIndex(e => e.Status);

            b.HasOne(e => e.Position)
                .WithMany()
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.Location)
                .WithMany()
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.Manager)
                .WithMany(m => m.DirectReports)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.NoAction);

            // Soft delete: IsDeleted=false olanlar default sorgularda görünür.
            b.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<SalaryBandEntity>(b =>
        {
            b.ToTable("SalaryBands");
            b.HasKey(e => e.Id);
            b.Property(e => e.Min).HasColumnType("decimal(18,2)");
            b.Property(e => e.Mid).HasColumnType("decimal(18,2)");
            b.Property(e => e.Max).HasColumnType("decimal(18,2)");
            b.HasOne(e => e.JobGrade).WithMany().HasForeignKey(e => e.JobGradeId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(e => e.Location).WithMany().HasForeignKey(e => e.LocationId).OnDelete(DeleteBehavior.SetNull);
            b.HasIndex(e => new { e.JobGradeId, e.LocationId, e.EffectiveDate });
        });

        modelBuilder.Entity<EmployeeSalaryEntity>(b =>
        {
            b.ToTable("EmployeeSalaries");
            b.HasKey(e => e.Id);
            b.Property(e => e.GrossMonthly).HasColumnType("decimal(18,2)");
            b.Property(e => e.NetMonthlyCached).HasColumnType("decimal(18,2)");
            b.Property(e => e.EmployerCostCached).HasColumnType("decimal(18,2)");
            b.Property(e => e.ChangePercent).HasColumnType("decimal(8,4)");
            b.Property(e => e.ChangeAmount).HasColumnType("decimal(18,2)");
            b.HasOne(e => e.Employee).WithMany().HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(e => new { e.EmployeeId, e.EffectiveDate });
        });

        modelBuilder.Entity<ScenarioEntity>(b =>
        {
            b.ToTable("Scenarios");
            b.HasKey(e => e.Id);
            b.Property(e => e.Name).HasMaxLength(256).IsRequired();
            b.Property(e => e.CreatedBy).HasMaxLength(256).IsRequired();
            b.Property(e => e.AppliedBy).HasMaxLength(256);
            b.Property(e => e.ParametersJson).HasColumnType("nvarchar(max)");
            b.HasIndex(e => e.CreatedAtUtc);
        });

        modelBuilder.Entity<ScenarioEmployeeEntity>(b =>
        {
            b.ToTable("ScenarioEmployees");
            b.HasKey(e => e.Id);
            b.Property(e => e.OldGross).HasColumnType("decimal(18,2)");
            b.Property(e => e.NewGross).HasColumnType("decimal(18,2)");
            b.Property(e => e.OldNetMonthly).HasColumnType("decimal(18,2)");
            b.Property(e => e.NewNetMonthly).HasColumnType("decimal(18,2)");
            b.Property(e => e.OldEmployerCost).HasColumnType("decimal(18,2)");
            b.Property(e => e.NewEmployerCost).HasColumnType("decimal(18,2)");
            b.Property(e => e.RaisePercent).HasColumnType("decimal(8,4)");
            b.Property(e => e.RaiseAmount).HasColumnType("decimal(18,2)");
            b.HasOne(e => e.Scenario).WithMany(s => s.Employees).HasForeignKey(e => e.ScenarioId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(e => e.Employee).WithMany().HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(e => new { e.ScenarioId, e.EmployeeId }).IsUnique();
        });
    }
}
