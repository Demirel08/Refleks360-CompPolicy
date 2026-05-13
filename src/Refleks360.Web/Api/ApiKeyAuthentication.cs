using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Refleks360.Web.Api;

public sealed class ApiKeyAuthOptions : AuthenticationSchemeOptions
{
    public const string Scheme = "ApiKey";
    public const string HeaderName = "X-Api-Key";
}

/// <summary>
/// X-Api-Key header'ı configuration "Api:Keys" listesinde varsa istek doğrulanır.
/// Sistem "api" rolünde davranır (sadece API endpoints'e erişim).
/// </summary>
public sealed class ApiKeyAuthHandler(IConfiguration configuration, IOptionsMonitor<ApiKeyAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : AuthenticationHandler<ApiKeyAuthOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyAuthOptions.HeaderName, out var keys) || keys.Count == 0)
            return Task.FromResult(AuthenticateResult.NoResult());

        var apiKey = keys.ToString();
        var allowed = configuration.GetSection("Api:Keys").Get<string[]>() ?? Array.Empty<string>();
        if (!allowed.Contains(apiKey))
            return Task.FromResult(AuthenticateResult.Fail("Geçersiz API key."));

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "api-client"),
            new Claim(ClaimTypes.Role, "ApiClient"),
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
