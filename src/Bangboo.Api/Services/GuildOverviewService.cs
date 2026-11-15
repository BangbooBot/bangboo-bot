using Bangboo.Api.Common.Discord;
using Bangboo.Api.Dtos;
using Bangboo.Api.Services.Interfaces;

namespace Bangboo.Api.Services;

public sealed class GuildOverviewService(IDiscordApiClient discord, IMousetrapService mousetrap) : IGuildOverviewService
{
    public async Task<GuildOverviewResponse> GetOverviewAsync(string guildId, CancellationToken ct)
    {
        var guild = await discord.GetGuildAsync(guildId, ct);
        var channels = await discord.GetGuildChannelsAsync(guildId, ct);
        var mousetrapSettings = await mousetrap.GetAsync(guildId, ct);

        return new GuildOverviewResponse(
            guild.Id,
            guild.Name,
            DiscordCdn.GuildIcon(guild.Id, guild.Icon),
            DiscordCdn.GuildBanner(guild.Id, guild.Banner),
            guild.Description,
            guild.OwnerId,
            DiscordSnowflake.ToTimestamp(guild.Id),
            guild.ApproximateMemberCount ?? 0,
            guild.ApproximatePresenceCount ?? 0,
            guild.PremiumTier,
            guild.PremiumSubscriptionCount ?? 0,
            channels.Count(GuildChannelMapper.IsTextLike),
            channels.Count(GuildChannelMapper.IsVoiceLike),
            channels.Count(channel => channel.Type == GuildChannelMapper.Category),
            // O cargo @everyone tem o mesmo ID da guild e não conta como cargo criado.
            guild.Roles?.Count(role => role.Id != guild.Id) ?? 0,
            guild.PreferredLocale,
            mousetrapSettings);
    }

    public async Task<IReadOnlyList<GuildChannelResponse>> GetTextChannelsAsync(string guildId, CancellationToken ct) =>
        GuildChannelMapper.ToTextChannels(await discord.GetGuildChannelsAsync(guildId, ct));

    public async Task<IReadOnlyList<GuildRoleResponse>> GetRolesAsync(string guildId, CancellationToken ct) =>
        GuildRoleMapper.ToAssignableRoles(await discord.GetGuildAsync(guildId, ct));
}
