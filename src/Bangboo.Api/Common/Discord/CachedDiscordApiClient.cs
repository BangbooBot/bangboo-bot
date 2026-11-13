using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace Bangboo.Api.Common.Discord;

/// <summary>
/// Decorator com cache em memória sobre <see cref="IDiscordApiClient"/>, para respeitar o
/// rate limit do Discord (o dashboard consulta as mesmas listas várias vezes por navegação).
/// </summary>
public sealed class CachedDiscordApiClient(IDiscordApiClient inner, IMemoryCache cache) : IDiscordApiClient
{
    private static readonly TimeSpan UserGuildsTtl = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan BotGuildsTtl = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan CommandsTtl = TimeSpan.FromMinutes(5);

    public Task<IReadOnlyList<DiscordPartialGuild>> GetUserGuildsAsync(string userAccessToken, CancellationToken ct) =>
        GetOrCreateAsync($"discord:user-guilds:{Hash(userAccessToken)}", UserGuildsTtl,
            () => inner.GetUserGuildsAsync(userAccessToken, ct));

    public Task<IReadOnlySet<string>> GetBotGuildIdsAsync(CancellationToken ct) =>
        GetOrCreateAsync("discord:bot-guilds", BotGuildsTtl, () => inner.GetBotGuildIdsAsync(ct));

    public Task<IReadOnlyList<DiscordApplicationCommand>> GetApplicationCommandsAsync(CancellationToken ct) =>
        GetOrCreateAsync("discord:commands", CommandsTtl, () => inner.GetApplicationCommandsAsync(ct));

    private async Task<T> GetOrCreateAsync<T>(string key, TimeSpan ttl, Func<Task<T>> factory)
    {
        if (cache.TryGetValue(key, out T? cached) && cached is not null)
        {
            return cached;
        }

        var value = await factory();
        cache.Set(key, value, ttl);
        return value;
    }

    // Não usa o token em claro como chave de cache.
    private static string Hash(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
