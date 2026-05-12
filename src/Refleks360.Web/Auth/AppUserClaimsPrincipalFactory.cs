using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Refleks360.Domain.Security;
using Refleks360.Infrastructure.Identity;

namespace Refleks360.Web.Auth;

/// <summary>
/// Identity cookie'sini imzalayan factory. Standart claim'lere ek olarak kullanıcının
/// rollerinden türetilen <c>permission</c> claim'lerini ekler. Böylece
/// <c>[Authorize(Policy = "Employee.View")]</c> doğrudan çalışır, ek DB sorgusu yok.
/// </summary>
public sealed class AppUserClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>(userManager, roleManager, optionsAccessor)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        var roleNames = await UserManager.GetRolesAsync(user);
        var permissions = new HashSet<string>(StringComparer.Ordinal);

        foreach (var role in roleNames)
        {
            if (Permissions.DefaultRolePermissions.TryGetValue(role, out var perms))
            {
                foreach (var p in perms) permissions.Add(p);
            }
        }

        foreach (var permission in permissions)
        {
            identity.AddClaim(new Claim(Permissions.ClaimType, permission));
        }

        return identity;
    }
}
