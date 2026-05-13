using Microsoft.Extensions.Caching.Memory;

namespace Refleks360.Infrastructure.Services;

/// <summary>
/// Yazma işlemi sonrası ilgili read-cache'lerin invalide edilmesini sağlayan yardımcı.
/// Şu an basit (key bazlı sileceği bilinen şeyler); ileride event-driven yapılabilir.
/// </summary>
public sealed class CacheInvalidator(IMemoryCache cache)
{
    public void InvalidateEmployees()
    {
        cache.Remove("employee-list-with-compa");
    }

    public void InvalidateLookups()
    {
        cache.Remove("lookup:departments");
        cache.Remove("lookup:positions");
        cache.Remove("lookup:locations");
        cache.Remove("lookup:grades");
        cache.Remove("lookup:families");
        // manager candidates anahtarları farklı; tek tek silmek pratik değil — TTL kısa (1 dk)
    }

    public void InvalidateAll()
    {
        InvalidateEmployees();
        InvalidateLookups();
    }
}
