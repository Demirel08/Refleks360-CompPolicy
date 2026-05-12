using Microsoft.AspNetCore.Identity;
using Refleks360.Domain.Security;
using Refleks360.Infrastructure.Identity;

namespace Refleks360.Web.Auth;

/// <summary>
/// İlk açılışta sabit bir admin hesabı oluşturur. Şifre <c>AdminSeed:Password</c>
/// (user-secrets) içinden alınır; eksikse <see cref="DefaultPassword"/> kullanılır
/// (yalnızca geliştirme).
/// </summary>
public static class AdminUserSeeder
{
    public const string AdminEmail = "admin@refleks360.local";
    public const string AdminUserName = "admin";
    private const string DefaultPassword = "Admin123!";

    public static async Task SeedAsync(IServiceProvider services, ILogger logger)
    {
        using var scope = services.CreateScope();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        // 8 sistem rolünü tohumla
        foreach (var roleName in Roles.All)
        {
            if (!await roleMgr.RoleExistsAsync(roleName))
            {
                var rr = await roleMgr.CreateAsync(new IdentityRole(roleName));
                if (!rr.Succeeded)
                {
                    logger.LogError("{Role} rolü oluşturulamadı: {Errors}",
                        roleName, string.Join("; ", rr.Errors.Select(e => e.Description)));
                }
                else
                {
                    logger.LogInformation("Rol oluşturuldu: {Role}", roleName);
                }
            }
        }

        var existing = await userMgr.FindByNameAsync(AdminUserName);
        if (existing is not null)
        {
            // Eski admin'i SystemAdmin rolüne yükselt (önceki sürümden geliyorsa).
            if (!await userMgr.IsInRoleAsync(existing, Roles.SystemAdmin))
            {
                await userMgr.AddToRoleAsync(existing, Roles.SystemAdmin);
                logger.LogInformation("Mevcut admin {User} SystemAdmin rolüne eklendi.", existing.UserName);
            }
            logger.LogInformation("Admin kullanıcı zaten var ({UserName}).", existing.UserName);
            return;
        }

        var password = config["AdminSeed:Password"];
        if (string.IsNullOrWhiteSpace(password))
        {
            password = DefaultPassword;
            logger.LogWarning(
                "AdminSeed:Password ayarlı değil; geliştirme varsayılanı '{DefaultPassword}' kullanılıyor. " +
                "Production'da user-secrets veya env-var ile değiştir.", DefaultPassword);
        }

        var user = new ApplicationUser
        {
            UserName = AdminUserName,
            Email = AdminEmail,
            EmailConfirmed = true,
            FullName = "Sistem Yöneticisi",
            IsActive = true,
        };

        var create = await userMgr.CreateAsync(user, password);
        if (!create.Succeeded)
        {
            logger.LogError("Admin kullanıcı oluşturulamadı: {Errors}",
                string.Join("; ", create.Errors.Select(e => e.Description)));
            return;
        }

        await userMgr.AddToRoleAsync(user, Roles.SystemAdmin);
        logger.LogInformation("Admin kullanıcı oluşturuldu: {UserName} (rol: {Role})", AdminUserName, Roles.SystemAdmin);
    }
}
