using Refleks360.Infrastructure.Persistence.Auditing;

namespace Refleks360.Web.Auth;

/// <summary>
/// <see cref="IAuditUserContext"/>'in HTTP istek başına kullanıcı adını
/// <see cref="IHttpContextAccessor"/>'tan çözen Web uygulaması karşılığı.
/// Arka plan job'ları (Hangfire) bu servisi göremez ve "system" döner.
/// </summary>
public sealed class HttpAuditUserContext(IHttpContextAccessor httpContextAccessor) : IAuditUserContext
{
    public string GetCurrentUserName()
    {
        var name = httpContextAccessor.HttpContext?.User?.Identity?.Name;
        return string.IsNullOrWhiteSpace(name) ? "system" : name;
    }
}
