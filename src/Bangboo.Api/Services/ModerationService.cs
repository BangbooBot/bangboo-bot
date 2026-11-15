using Bangboo.Api.Common.Discord;
using Bangboo.Api.Common.Errors;
using Bangboo.Api.Dtos;
using Bangboo.Api.Services.Interfaces;
using Bangboo.Core.Data;
using Bangboo.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bangboo.Api.Services;

public sealed class ModerationService(AppDbContext db, IDiscordApiClient discord) : IModerationService
{
    public async Task<ModerationResponse> GetAsync(string guildId, CancellationToken ct)
    {
        var id = ParseGuildId(guildId);
        // Confere que o bot está na guild (404 caso contrário), como as outras páginas do painel.
        await discord.GetGuildAsync(guildId, ct);

        var roleIds = await db.ModeratorRoles.AsNoTracking()
            .Where(r => r.GuildId == id)
            .Select(r => r.RoleId)
            .ToListAsync(ct);

        return ToResponse(roleIds);
    }

    public async Task<ModerationResponse> UpdateAsync(string guildId, UpdateModerationRequest request, CancellationToken ct)
    {
        var id = ParseGuildId(guildId);
        var guild = await discord.GetGuildAsync(guildId, ct);
        var roleIds = Validate(request, guild);

        await db.UpsertGuildAsync(id, guild, ct);

        var current = await db.ModeratorRoles.Where(r => r.GuildId == id).ToListAsync(ct);
        db.ModeratorRoles.RemoveRange(current.Where(r => !roleIds.Contains(r.RoleId)));

        var existing = current.Select(r => r.RoleId).ToHashSet();
        db.ModeratorRoles.AddRange(roleIds
            .Where(roleId => !existing.Contains(roleId))
            .Select(roleId => new ModeratorRole { GuildId = id, RoleId = roleId }));

        await db.SaveChangesAsync(ct);
        return ToResponse(roleIds);
    }

    /// <summary>Todo ID precisa ser um cargo atribuível da guild; devolve os IDs sem repetição.</summary>
    private static HashSet<ulong> Validate(UpdateModerationRequest request, DiscordGuild guild)
    {
        var assignable = (guild.Roles ?? [])
            .Where(role => GuildRoleMapper.IsAssignable(role, guild.Id))
            .Select(role => role.Id)
            .ToHashSet(StringComparer.Ordinal);

        var roleIds = new HashSet<ulong>();
        foreach (var roleId in request.RoleIds)
        {
            if (!assignable.Contains(roleId) || !ulong.TryParse(roleId, out var parsed))
            {
                throw new ValidationException("One of the selected roles does not exist in this server or cannot be assigned");
            }
            roleIds.Add(parsed);
        }

        return roleIds;
    }

    private static ModerationResponse ToResponse(IEnumerable<ulong> roleIds) =>
        new(roleIds.Order().Select(roleId => roleId.ToString()).ToList());

    private static ulong ParseGuildId(string guildId) =>
        ulong.TryParse(guildId, out var id) ? id : throw new NotFoundException("Guild not found");
}
