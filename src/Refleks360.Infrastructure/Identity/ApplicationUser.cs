using Microsoft.AspNetCore.Identity;

namespace Refleks360.Infrastructure.Identity;

/// <summary>
/// Refleks 360 ÜP kullanıcı kimliği. <see cref="IdentityUser"/>'a şirket içi
/// alanlar (FullName, son giriş, vs.) ileride eklenir.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>Tam ad (kayıt sırasında zorunlu).</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Hesap aktif mi? Pasif kullanıcı giriş yapamaz.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Son başarılı giriş zamanı (audit + son aktivite).</summary>
    public DateTimeOffset? LastLoginAt { get; set; }
}
