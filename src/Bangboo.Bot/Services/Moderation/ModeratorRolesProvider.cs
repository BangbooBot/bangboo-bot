using System.Collections.Concurrent;
using Bangboo.Core.Data;
using Bot.Handler;
using Microsoft.EntityFrameworkCore;
using NetCord;

namespace Bot.Services.Moderation;

/// <summary>Cargos que podem usar o <c>/moderate</c> numa guild (salvos pelo dashboard).</summary>
public interface IModeratorRolesProvider
{
    Task<IReadOnlySet<ulong>> GetAsync(ulong guildId, CancellationToken ct);
}

/// <summary>Lê do banco com cache curto. Mudanças feitas no dashboard valem em até <see cref="CacheTtl"/>.</summary>
public sealed class ModeratorRolesProvider(IDbContextFactory<AppDbContext> dbFactory, TimeProvider clock)
    : IModeratorRolesProvider
{
    public static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(30);

    private readonly ConcurrentDictionary<ulong, (IReadOnlySet<ulong> RoleIds, DateTimeOffset ExpiresAt)> _cache = new();

    public async Task<IReadOnlySet<ulong>> GetAsync(ulong guildId, CancellationToken ct)
    {
        var now = clock.GetUtcNow();
        if (_cache.TryGetValue(guildId, out var cached) && cached.ExpiresAt > now)
        {
            return cached.RoleIds;
        }

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var roleIds = await db.ModeratorRoles.AsNoTracking()
            .Where(r => r.GuildId == guildId)
            .Select(r => r.RoleId)
            .ToListAsync(ct);

        IReadOnlySet<ulong> result = roleIds.ToHashSet();
        _cache[guildId] = (result, now + CacheTtl);
        return result;
    }
}

public static class ModeratorRolesProviderExtensions
{
    /// <summary>Lança <see cref="BotException"/> se o usuário não puder usar o <c>/moderate</c> nesta guild.</summary>
    public static async Task EnsureCanModerateAsync(
        this IModeratorRolesProvider provider,
        NetCord.Gateway.Guild? guild,
        User user,
        CancellationToken ct = default)
    {
        if (guild is null || user is not GuildInteractionUser member)
        {
            throw new BotException("This command can only be used in a server.");
        }

        var allowed = await provider.GetAsync(guild.Id, ct);
        var canModerate = ModerationPolicy.CanModerate(
            isOwner: guild.OwnerId == member.Id,
            isAdministrator: member.Permissions.HasFlag(Permissions.Administrator),
            member.RoleIds,
            allowed);

        if (!canModerate)
        {
            throw new BotException("You don't have a moderator role in this server.");
        }
    }
}
