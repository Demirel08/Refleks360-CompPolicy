using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refleks360.Application.Abstractions;
using Refleks360.Infrastructure.Identity;
using Refleks360.Infrastructure.Persistence;
using Refleks360.Infrastructure.Persistence.Auditing;
using Refleks360.Infrastructure.Services;

namespace Refleks360.Infrastructure;

/// <summary>
/// Refleks 360 Infrastructure katmanını DI'a kaydeden uzantılar.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// <see cref="CompDbContext"/>'i, Identity'yi ve uygulama servislerini kaydeder.
    /// Caller <c>AddMemoryCache()</c>'i ayrıca çağırmalı.
    /// </summary>
    public static IServiceCollection AddRefleks360Infrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException(
                "ConnectionStrings:Default ayarlı değil. user-secrets veya appsettings içinden ekle.");

        services.AddDbContext<CompDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(typeof(CompDbContext).Assembly.FullName));
            options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        services.AddScoped<AuditSaveChangesInterceptor>();

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Geliştirme — production'da sıkılaştırılır (NOTLAR'da belgelenecek)
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddEntityFrameworkStores<CompDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<ITaxParameterService, TaxParameterService>();
        services.AddScoped<IEmployeeQueryService, EmployeeQueryService>();
        services.AddScoped<IOrganizationLookupService, OrganizationLookupService>();
        services.AddScoped<IUserAdminService, UserAdminService>();
        services.AddScoped<IEmployeeImportService, EmployeeImportService>();
        services.AddScoped<IOrganizationAdminService, OrganizationAdminService>();
        services.AddScoped<ISalaryBandService, SalaryBandService>();
        services.AddScoped<ICompPolicyMetricsService, CompPolicyMetricsService>();
        services.AddScoped<IScenarioService, ScenarioService>();
        services.AddScoped<ISimulationService, SimulationService>();
        services.AddScoped<IDashboardService, DashboardService>();

        // MediatR Infrastructure handler'larını da tara.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
