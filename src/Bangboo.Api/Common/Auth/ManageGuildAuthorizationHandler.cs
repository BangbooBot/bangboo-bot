using Bangboo.Api.Common.Discord;
using Microsoft.AspNetCore.Authorization;

namespace Bangboo.Api.Common.Auth;

public sealed class ManageGuildRequirement : IAuthorizationRequirement;

/// <summary>
/// Autoriza rotas <c>api/guilds/{guildId}/...</c> se o usuário pode gerenciar a guild
/// segundo o Discord.
/// </summary>
public sealed class ManageGuildAuthorizationHandler(ICurrentUser currentUser, IDiscordApiClient discord)
    : AuthorizationHandler<ManageGuildRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ManageGuildRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true ||
            context.Resource is not HttpContext httpContext ||
            httpContext.GetRouteValue(AuthPolicies.GuildIdRouteValue) is not string guildId)
        {
            return;
        }

        var accessToken = await currentUser.GetDiscordAccessTokenAsync();
        var guilds = await discord.GetUserGuildsAsync(accessToken, httpContext.RequestAborted);

        if (guilds.Any(guild => guild.Id == guildId && DiscordPermissions.CanManage(guild)))
        {
            context.Succeed(requirement);
        }
    }
}
