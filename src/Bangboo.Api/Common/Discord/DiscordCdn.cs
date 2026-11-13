namespace Bangboo.Api.Common.Discord;

/// <summary>Monta URLs de imagens do CDN do Discord.</summary>
public static class DiscordCdn
{
    private const string BaseUrl = "https://cdn.discordapp.com";

    public static string UserAvatar(string userId, string? avatarHash)
    {
        if (string.IsNullOrEmpty(avatarHash))
        {
            // Avatares padrão: índice = (id >> 22) % 6 para usernames novos.
            var index = ulong.TryParse(userId, out var id) ? (id >> 22) % 6 : 0;
            return $"{BaseUrl}/embed/avatars/{index}.png";
        }

        var extension = avatarHash.StartsWith("a_", StringComparison.Ordinal) ? "gif" : "png";
        return $"{BaseUrl}/avatars/{userId}/{avatarHash}.{extension}";
    }

    public static string? GuildIcon(string guildId, string? iconHash) =>
        string.IsNullOrEmpty(iconHash) ? null : $"{BaseUrl}/icons/{guildId}/{iconHash}.png";

    public static string? GuildBanner(string guildId, string? bannerHash) =>
        string.IsNullOrEmpty(bannerHash) ? null : $"{BaseUrl}/banners/{guildId}/{bannerHash}.png";
}
