using System.ComponentModel.DataAnnotations;
using Bangboo.Core.Entities;

namespace Bangboo.Api.Dtos;

/// <summary>Cargo atribuível da guild (para seletores do dashboard).</summary>
/// <param name="Id">ID do cargo.</param>
/// <param name="Name">Nome.</param>
/// <param name="Color">Cor em RGB inteiro (0 = sem cor).</param>
public sealed record GuildRoleResponse(string Id, string Name, int Color);

/// <summary>Quem pode usar o comando <c>/moderate</c>.</summary>
/// <param name="RoleIds">Cargos liberados. O dono e administradores sempre podem.</param>
public sealed record ModerationResponse(IReadOnlyList<string> RoleIds);

/// <summary>Novos cargos que podem usar o comando <c>/moderate</c> (substitui a lista atual).</summary>
/// <param name="RoleIds">IDs de cargos atribuíveis da guild (até 25).</param>
public sealed record UpdateModerationRequest(
    [Required, MaxLength(ModeratorRole.MaxPerGuild)] IReadOnlyList<string> RoleIds);
