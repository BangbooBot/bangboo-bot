using System.Security.Claims;
using Bangboo.Api.Common.Errors;
using Microsoft.AspNetCore.Authentication;

namespace Bangboo.Api.Common.Auth;

public sealed class HttpCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private HttpContext Context => accessor.HttpContext
        ?? throw new InvalidOperationException("ICurrentUser used outside of an HTTP request.");

    private ClaimsPrincipal Principal => Context.User.Identity?.IsAuthenticated == true
        ? Context.User
        : throw new UnauthorizedException();

    public string Id => Principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedException();

    public string Username => Principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

    public string? GlobalName => Principal.FindFirstValue(BangbooClaimTypes.GlobalName);

    public string? AvatarHash => Principal.FindFirstValue(BangbooClaimTypes.Avatar);

    public async Task<string> GetDiscordAccessTokenAsync() =>
        await Context.GetTokenAsync("access_token")
        ?? throw new UnauthorizedException("Discord session expired, please log in again");
}
