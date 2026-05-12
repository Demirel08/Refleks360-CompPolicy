using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refleks360.Application.Abstractions;
using Refleks360.Infrastructure.Persistence;
using Refleks360.Infrastructure.Services;

namespace Refleks360.Infrastructure;

/// <summary>
/// Refleks 360 Infrastructure katmanını DI'a kaydeden uzantılar.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// <see cref="CompDbContext"/>'i (<c>ConnectionStrings:Default</c> üzerinden) ve
    /// uygulama servislerini kaydeder. <see cref="IMemoryCache"/> ihtiyacı için
    /// caller <c>AddMemoryCache()</c>'i çağırmış olmalı.
    /// </summary>
    public static IServiceCollection AddRefleks360Infrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException(
                "ConnectionStrings:Default ayarlı değil. user-secrets veya appsettings içinden ekle.");

        services.AddDbContext<CompDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(typeof(CompDbContext).Assembly.FullName)));

        services.AddScoped<ITaxParameterService, TaxParameterService>();

        return services;
    }
}
