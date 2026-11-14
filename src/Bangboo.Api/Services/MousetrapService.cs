using Bangboo.Api.Common.Discord;
using Bangboo.Api.Common.Errors;
using Bangboo.Api.Dtos;
using Bangboo.Api.Services.Interfaces;
using Bangboo.Core.Data;
using Bangboo.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bangboo.Api.Services;

public sealed class MousetrapService(AppDbContext db, IDiscordApiClient discord) : IMousetrapService
{
    public async Task<MousetrapResponse> GetAsync(string guildId, CancellationToken ct)
    {
        var id = ParseGuildId(guildId);
        var mousetrap = await db.Mousetraps.AsNoTracking().FirstOrDefaultAsync(m => m.GuildId == id, ct);
        var channels = await discord.GetGuildChannelsAsync(guildId, ct);

        return ToResponse(mousetrap ?? new Mousetrap { GuildId = id }, channels);
    }

    public async Task<MousetrapResponse> UpdateAsync(string guildId, UpdateMousetrapRequest request, CancellationToken ct)
    {
        var id = ParseGuildId(guildId);
        var guild = await discord.GetGuildAsync(guildId, ct);
        var channels = await discord.GetGuildChannelsAsync(guildId, ct);
        var channelId = Validate(request, channels);

        await UpsertGuildAsync(id, guild, ct);

        var mousetrap = await db.Mousetraps.FirstOrDefaultAsync(m => m.GuildId == id, ct);
        if (mousetrap is null)
        {
            mousetrap = new Mousetrap { GuildId = id };
            db.Mousetraps.Add(mousetrap);
        }

        mousetrap.Enabled = request.Enabled;
        mousetrap.ChannelId = channelId;
        mousetrap.Action = request.Action;
        mousetrap.MinTenureDays = request.Action == MousetrapAction.ByTenure ? request.MinTenureDays : null;
        mousetrap.SendDm = request.SendDm;
        mousetrap.DmMessage = string.IsNullOrWhiteSpace(request.DmMessage) ? null : request.DmMessage.Trim();

        await db.SaveChangesAsync(ct);
        return ToResponse(mousetrap, channels);
    }

    /// <summary>Regras entre campos; devolve o canal já convertido (ou null).</summary>
    private static ulong? Validate(UpdateMousetrapRequest request, IReadOnlyList<DiscordChannel> channels)
    {
        if (!Enum.IsDefined(request.Action))
        {
            throw new ValidationException("Invalid action");
        }

        ulong? channelId = null;
        if (!string.IsNullOrWhiteSpace(request.ChannelId))
        {
            var channel = channels.FirstOrDefault(c => c.Id == request.ChannelId);
            if (channel is null || !GuildChannelMapper.IsTextLike(channel) || !ulong.TryParse(channel.Id, out var parsed))
            {
                throw new ValidationException("The selected channel is not a text channel of this server");
            }
            channelId = parsed;
        }

        if (request.Enabled && channelId is null)
        {
            throw new ValidationException("Select a channel to enable the mousetrap");
        }

        if (request.Action == MousetrapAction.ByTenure && request.MinTenureDays is null)
        {
            throw new ValidationException("Set the minimum tenure in days for the by-tenure action");
        }

        if (request.SendDm && string.IsNullOrWhiteSpace(request.DmMessage))
        {
            throw new ValidationException("Write the message to send or turn off the message");
        }

        return channelId;
    }

    /// <summary>A guild precisa existir na tabela <c>guilds</c> (FK do mousetrap).</summary>
    private async Task UpsertGuildAsync(ulong id, DiscordGuild discordGuild, CancellationToken ct)
    {
        var guild = await db.Guilds.FirstOrDefaultAsync(g => g.Id == id, ct);
        if (guild is null)
        {
            guild = new Guild { Id = id, Name = discordGuild.Name };
            db.Guilds.Add(guild);
        }

        guild.Name = discordGuild.Name;
        guild.Icon = discordGuild.Icon;
        guild.Banner = discordGuild.Banner;
    }

    private static MousetrapResponse ToResponse(Mousetrap mousetrap, IReadOnlyList<DiscordChannel> channels)
    {
        var channelId = mousetrap.ChannelId?.ToString();
        var channelName = channelId is null ? null : channels.FirstOrDefault(c => c.Id == channelId)?.Name;

        return new MousetrapResponse(
            mousetrap.Enabled,
            channelId,
            channelName,
            mousetrap.Action,
            mousetrap.MinTenureDays,
            mousetrap.SendDm,
            mousetrap.DmMessage);
    }

    private static ulong ParseGuildId(string guildId) =>
        ulong.TryParse(guildId, out var id) ? id : throw new NotFoundException("Guild not found");
}
