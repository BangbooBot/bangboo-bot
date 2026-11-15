using Bangboo.Api.Common.Discord;
using Bangboo.Api.Dtos;

namespace Bangboo.Api.Services;

/// <summary>Filtra e ordena os cargos de uma guild como o Discord mostra (do mais alto para o mais baixo).</summary>
public static class GuildRoleMapper
{
    /// <summary>
    /// Cargos que podem ser dados a membros: sem o <c>@everyone</c> (mesmo ID da guild)
    /// e sem cargos gerenciados por bots/integrações.
    /// </summary>
    public static bool IsAssignable(DiscordRole role, string guildId) =>
        role.Id != guildId && !role.Managed;

    public static IReadOnlyList<GuildRoleResponse> ToAssignableRoles(DiscordGuild guild) =>
        (guild.Roles ?? [])
            .Where(role => IsAssignable(role, guild.Id))
            .OrderByDescending(role => role.Position)
            .ThenBy(role => role.Id, StringComparer.Ordinal)
            .Select(role => new GuildRoleResponse(role.Id, role.Name, role.Color))
            .ToList();
}
