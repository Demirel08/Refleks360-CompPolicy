using Microsoft.AspNetCore.Identity;
using Refleks360.Infrastructure.Identity;

namespace Refleks360.Web.Auth;

/// <summary>
/// Cookie tabanlı login/logout için minimal API uç noktaları. Blazor Server
/// interaktif devresi cookie yazamaz, bu yüzden form post → endpoint yolu kullanılır.
/// </summary>
public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth").DisableAntiforgery(); // form'da kendi token'ımız olmadığı için

        group.MapPost("/login", async (
            HttpContext http,
            SignInManager<ApplicationUser> signIn,
            UserManager<ApplicationUser> users,
            ILoggerFactory loggerFactory) =>
        {
            var form = await http.Request.ReadFormAsync();
            var userName = form["userName"].ToString().Trim();
            var password = form["password"].ToString();
            var returnUrl = form["returnUrl"].ToString();
            if (string.IsNullOrWhiteSpace(returnUrl)) returnUrl = "/";

            var logger = loggerFactory.CreateLogger("AuthEndpoints.Login");

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                return Results.Redirect("/login?error=missing&returnUrl=" + Uri.EscapeDataString(returnUrl));
            }

            var user = await users.FindByNameAsync(userName);
            if (user is null || !user.IsActive)
            {
                logger.LogWarning("Başarısız giriş: kullanıcı yok veya pasif ({UserName}).", userName);
                return Results.Redirect("/login?error=invalid&returnUrl=" + Uri.EscapeDataString(returnUrl));
            }

            var result = await signIn.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                var reason = result.IsLockedOut ? "locked"
                           : result.IsNotAllowed ? "notallowed"
                           : "invalid";
                logger.LogWarning("Başarısız giriş ({Reason}): {UserName}", reason, userName);
                return Results.Redirect("/login?error=" + reason + "&returnUrl=" + Uri.EscapeDataString(returnUrl));
            }

            user.LastLoginAt = DateTimeOffset.UtcNow;
            await users.UpdateAsync(user);

            logger.LogInformation("Giriş başarılı: {UserName}", userName);
            return Results.Redirect(returnUrl);
        });

        group.MapPost("/logout", async (
            HttpContext http,
            SignInManager<ApplicationUser> signIn) =>
        {
            await signIn.SignOutAsync();
            return Results.Redirect("/login");
        });

        return endpoints;
    }
}
