using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Refleks360.Application;

/// <summary>
/// Application katmanını DI'a kaydeden uzantı (MediatR handlers + FluentValidation
/// validators ortak otomatik discovery).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddRefleks360Application(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // MediatR handler'lar Infrastructure'da da olabilir; o kayıt orada yapılır.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
