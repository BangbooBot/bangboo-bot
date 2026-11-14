using Bangboo.Api.Common.Discord;
using Bangboo.Api.Dtos;

namespace Bangboo.Api.Services;

/// <summary>Classifica e ordena canais do Discord como o cliente mostra (categoria → posição).</summary>
public static class GuildChannelMapper
{
    public const int TextChannel = 0;
    public const int VoiceChannel = 2;
    public const int Category = 4;
    public const int AnnouncementChannel = 5;
    public const int StageChannel = 13;

    public static bool IsTextLike(DiscordChannel channel) =>
        channel.Type is TextChannel or AnnouncementChannel;

    public static bool IsVoiceLike(DiscordChannel channel) =>
        channel.Type is VoiceChannel or StageChannel;

    /// <summary>Canais onde se pode mandar mensagem (texto e anúncios), na ordem do Discord.</summary>
    public static IReadOnlyList<GuildChannelResponse> ToTextChannels(IReadOnlyList<DiscordChannel> channels)
    {
        var categories = channels
            .Where(channel => channel.Type == Category)
            .ToDictionary(channel => channel.Id);

        return channels
            .Where(IsTextLike)
            .OrderBy(channel => channel.ParentId is not null && categories.TryGetValue(channel.ParentId, out var parent)
                ? parent.Position
                : -1) // sem categoria vêm primeiro, como no Discord
            .ThenBy(channel => channel.Position)
            .Select(channel => new GuildChannelResponse(
                channel.Id,
                channel.Name ?? channel.Id,
                channel.ParentId is not null && categories.TryGetValue(channel.ParentId, out var category)
                    ? category.Name
                    : null))
            .ToList();
    }
}
