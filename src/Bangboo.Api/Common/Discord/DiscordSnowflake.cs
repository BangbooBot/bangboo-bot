namespace Bangboo.Api.Common.Discord;

/// <summary>Utilidades para IDs (snowflakes) do Discord.</summary>
public static class DiscordSnowflake
{
    // Época do Discord: 2015-01-01T00:00:00Z, em milissegundos.
    private const long DiscordEpochMs = 1420070400000;

    /// <summary>Momento de criação codificado no snowflake.</summary>
    public static DateTimeOffset ToTimestamp(string snowflake) =>
        ulong.TryParse(snowflake, out var id)
            ? DateTimeOffset.FromUnixTimeMilliseconds((long)(id >> 22) + DiscordEpochMs)
            : DateTimeOffset.UnixEpoch;
}
