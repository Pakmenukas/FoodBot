using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace FoodBot.Api.Auth;

public sealed class ApiKeyAuthenticationHandler(
    IOptionsMonitor<ApiKeyAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ApiKeyValidator validator)
    : AuthenticationHandler<ApiKeyAuthenticationOptions>(options, logger, encoder)
{
    private readonly ApiKeyAuthenticationOptions _options = options.Get(FoodBotAuthScheme.ApiKeyScheme);

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var raw = TryReadRawApiKey();
        if (string.IsNullOrWhiteSpace(raw))
            return AuthenticateResult.NoResult();

        if (!TryParseIdSecret(raw, out var id, out var secret))
        {
            return AuthenticateResult.Fail("Invalid API key format. Expected 'ApiKey {id}:{secret}' or header 'X-Api-Key: {id}:{secret}'.");
        }

        var validation = validator.ValidateAsync(id!, secret!);
        if (validation.IsFailure)
            return AuthenticateResult.Fail("Invalid API key.");

        var claims = new List<Claim>
        {
            new("UserId", id!), // TODO: constant
            new("PermissionLevel", ""), // TODO: constant
        };

        var identity = new ClaimsIdentity(claims, _options.DefaultScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, _options.DefaultScheme);
        return AuthenticateResult.Success(ticket);
    }

    private string? TryReadRawApiKey()
    {
        if (Request.Headers.TryGetValue("Authorization", out var authValues))
        {
            var auth = authValues.ToString();
            if (!string.IsNullOrWhiteSpace(auth))
            {
                return auth;
            }
        }

        if (Request.Headers.TryGetValue(_options.ApiKeySchemeHeader, out var keyValues))
        {
            var key = keyValues.ToString();
            if (!string.IsNullOrWhiteSpace(key))
            {
                return key;
            }
        }

        return null;
    }

    private static bool TryParseIdSecret(string raw, out string? id, out string? secret)
    {
        id = null;
        secret = null;

        var colonIdx = raw.IndexOf(':');
        if (colonIdx <= 0 || colonIdx >= raw.Length - 1)
            return false;

        id = raw[..colonIdx].Trim();
        secret = raw[(colonIdx + 1)..].Trim();
        return !string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(secret);
    }
}