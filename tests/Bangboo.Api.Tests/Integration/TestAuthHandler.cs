using System.Security.Claims;
using System.Text.Encodings.Web;
using Bangboo.Api.Common.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bangboo.Api.Tests.Integration;

/// <summary>
/// Substitui o cookie nos testes: a requisição é autenticada quando tem o header
/// <see cref="UserIdHeader"/>. Nunca chama o Discord.
/// </summary>
public sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Test";
    public const string UserIdHeader = "X-Test-UserId";
    public const string AccessToken = "test-access-token";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(UserIdHeader, out var userId))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, "tester"),
            new Claim(BangbooClaimTypes.GlobalName, "Tester"),
        ], SchemeName);

        var properties = new AuthenticationProperties();
        properties.StoreTokens([new AuthenticationToken { Name = "access_token", Value = AccessToken }]);

        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), properties, SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
