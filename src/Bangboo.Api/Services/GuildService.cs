using Bangboo.Api.Common.Auth;
using Bangboo.Api.Common.Discord;
using Bangboo.Api.Common.Errors;
using Bangboo.Api.Dtos;

using Bangboo.Api.Services.Interfaces;

namespace Bangboo.Api.Services;

public sealed class GuildService(ICurrentUser currentUser, IDiscordApiClient discord) : IGuildService
{
    public async Task<IReadOnlyList<GuildResponse>> GetManageableGuildsAsync(CancellationToken ct)
    {
        var accessToken = await currentUser.GetDiscordAccessTokenAsync();
        var userGuilds = await discord.GetUserGuildsAsync(accessToken, ct);
        var botGuildIds = await discord.GetBotGuildIdsAsync(ct);

        return userGuilds
            .Where(DiscordPermissions.CanManage)
            .Select(guild => ToResponse(guild, botGuildIds.Contains(guild.Id)))
            .OrderByDescending(guild => guild.IsBotMember)
            .ThenBy(guild => guild.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<GuildResponse> GetGuildAsync(string guildId, CancellationToken ct)
    {
        var guilds = await GetManageableGuildsAsync(ct);
        return guilds.FirstOrDefault(guild => guild.Id == guildId)
               ?? throw new NotFoundException("Guild not found");
    }

    private static GuildResponse ToResponse(DiscordPartialGuild guild, bool isBotMember) => new(
        guild.Id,
        guild.Name,
        DiscordCdn.GuildIcon(guild.Id, guild.Icon),
        DiscordCdn.GuildBanner(guild.Id, guild.Banner),
        guild.Owner,
        isBotMember,
        guild.ApproximateMemberCount ?? 0,
        guild.ApproximatePresenceCount ?? 0);
}
