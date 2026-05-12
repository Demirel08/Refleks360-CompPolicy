using Microsoft.AspNetCore.Authorization;
using Refleks360.Domain.Security;

namespace Refleks360.Web.Auth;

/// <summary>
/// "Belirli bir izne sahip olma" gereksinimi.
/// </summary>
public sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}

/// <summary>
/// Kullanıcının <see cref="Permissions.ClaimType"/> claim'i içinde gerekli izin
/// kodunun olup olmadığına bakar. SystemAdmin rolündeki kullanıcı her zaman izinli.
/// </summary>
public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.IsInRole(Roles.SystemAdmin))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var hasClaim = context.User.HasClaim(Permissions.ClaimType, requirement.Permission);
        if (hasClaim)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Her izin kodu için dinamik policy üretir; <c>[Authorize(Policy = "Employee.View")]</c>
/// gibi kullanım sağlar.
/// </summary>
public sealed class PermissionPolicyProvider(Microsoft.Extensions.Options.IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var existing = await base.GetPolicyAsync(policyName);
        if (existing is not null) return existing;

        // Permissions.All içindeki herhangi bir kod ise dinamik policy üret.
        if (Permissions.All.Contains(policyName))
        {
            return new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();
        }

        return null;
    }
}
