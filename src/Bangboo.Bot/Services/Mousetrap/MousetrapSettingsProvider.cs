using System.Collections.Concurrent;
using Bangboo.Core.Data;
using Microsoft.EntityFrameworkCore;
using MousetrapEntity = Bangboo.Core.Entities.Mousetrap;

namespace Bot.Services.Mousetrap;

/// <summary>Configuração do mousetrap de uma guild (salva pelo dashboard).</summary>
public interface IMousetrapSettingsProvider
{
    /// <summary>Configuração atual, ou null se a guild nunca configurou.</summary>
    Task<MousetrapEntity?> GetAsync(ulong guildId, CancellationToken ct);
}

/// <summary>
/// Lê do banco com cache curto: o handler roda a cada mensagem de todos os servidores, então
/// não pode ir ao banco toda vez. Mudanças feitas no dashboard valem em até <see cref="CacheTtl"/>.
/// </summary>
public sealed class MousetrapSettingsProvider(IDbContextFactory<AppDbContext> dbFactory, TimeProvider clock)
    : IMousetrapSettingsProvider
{
    public static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(30);

    private readonly ConcurrentDictionary<ulong, (MousetrapEntity? Settings, DateTimeOffset ExpiresAt)> _cache = new();

    public async Task<MousetrapEntity?> GetAsync(ulong guildId, CancellationToken ct)
    {
        var now = clock.GetUtcNow();
        if (_cache.TryGetValue(guildId, out var cached) && cached.ExpiresAt > now)
        {
            return cached.Settings;
        }

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var settings = await db.Mousetraps.AsNoTracking().FirstOrDefaultAsync(m => m.GuildId == guildId, ct);

        _cache[guildId] = (settings, now + CacheTtl);
        return settings;
    }
}
