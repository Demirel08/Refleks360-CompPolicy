using Microsoft.AspNetCore.Identity;
using Refleks360.Infrastructure.Identity;

namespace Refleks360.Web.Auth;

/// <summary>
/// İlk açılışta sabit bir admin hesabı oluşturur. Şifre <c>AdminSeed:Password</c>
/// (user-secrets) içinden alınır; eksikse <see cref="DefaultPassword"/> kullanılır
/// (yalnızca geliştirme).
/// </summary>
public static class AdminUserSeeder
{
    public const string AdminRole = "Admin";
    public const string AdminEmail = "admin@refleks360.local";
    public const string AdminUserName = "admin";
    private const string DefaultPassword = "Admin123!";

    public static async Task SeedAsync(IServiceProvider services, ILogger logger)
    {
        using var scope = services.CreateScope();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        if (!await roleMgr.RoleExistsAsync(AdminRole))
        {
            var rr = await roleMgr.CreateAsync(new IdentityRole(AdminRole));
            if (!rr.Succeeded)
            {
                logger.LogError("Admin rolü oluşturulamadı: {Errors}", string.Join("; ", rr.Errors.Select(e => e.Description)));
                return;
            }
            logger.LogInformation("Admin rolü oluşturuldu.");
        }

        var existing = await userMgr.FindByNameAsync(AdminUserName);
        if (existing is not null)
        {
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

        await userMgr.AddToRoleAsync(user, AdminRole);
        logger.LogInformation("Admin kullanıcı oluşturuldu: {UserName} (rol: {Role})", AdminUserName, AdminRole);
    }
}
