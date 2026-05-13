using Hangfire.Dashboard;

namespace Refleks360.Web.Auth;

/// <summary>Hangfire dashboard'a sadece SystemAdmin erişimine izin verir.</summary>
public sealed class HangfireAdminAuthFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var http = context.GetHttpContext();
        return http.User.Identity?.IsAuthenticated == true &&
               http.User.IsInRole(Refleks360.Domain.Security.Roles.SystemAdmin);
    }
}
