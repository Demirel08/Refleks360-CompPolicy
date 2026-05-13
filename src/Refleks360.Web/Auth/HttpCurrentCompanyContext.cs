using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Abstractions;
using Refleks360.Infrastructure.Persistence;

namespace Refleks360.Web.Auth;

/// <summary>
/// HTTP cookie tabanlı şirket seçimi. Default: kullanıcının erişebildiği ilk şirket
/// (şu an tek şirket seed'li olduğu için 1). Cookie adı: "Refleks360.Company".
/// </summary>
public sealed class HttpCurrentCompanyContext(IHttpContextAccessor http, CompDbContext db) : ICurrentCompanyContext
{
    private const string CookieName = "Refleks360.Company";

    public int CurrentCompanyId
    {
        get
        {
            var ctx = http.HttpContext;
            if (ctx is null) return 1;
            if (ctx.Request.Cookies.TryGetValue(CookieName, out var v) && int.TryParse(v, out var id))
                return id;
            return 1;
        }
    }

    public Task SetCurrentCompanyAsync(int companyId, CancellationToken ct = default)
    {
        var ctx = http.HttpContext ?? throw new InvalidOperationException("HttpContext yok.");
        ctx.Response.Cookies.Append(CookieName, companyId.ToString(),
            new CookieOptions { Path = "/", HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = DateTimeOffset.UtcNow.AddDays(30) });
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<CompanyOption>> GetAccessibleCompaniesAsync(CancellationToken ct = default)
    {
        // Şu an basitleştirilmiş: aktif tüm şirketler. Ileride UserCompanyAccess tablosu ile sinirlanabilir.
        return await db.Companies.AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new CompanyOption(c.Id, c.Name))
            .ToListAsync(ct);
    }
}
