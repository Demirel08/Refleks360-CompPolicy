namespace Refleks360.Infrastructure.Persistence.Auditing;

/// <summary>
/// Audit log için "şu an oturum açık kullanıcı" bilgisini sağlar. Web katmanı
/// <see cref="Microsoft.AspNetCore.Http.IHttpContextAccessor"/> üzerinden bunu
/// karşılar; arka plan job'ları sabit "system" değeri kullanabilir.
/// </summary>
public interface IAuditUserContext
{
    /// <summary>Şu anki kullanıcı adı (yoksa <c>"system"</c>).</summary>
    string GetCurrentUserName();
}
