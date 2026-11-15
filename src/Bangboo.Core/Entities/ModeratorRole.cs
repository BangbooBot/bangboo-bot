namespace Bangboo.Core.Entities;

/// <summary>
/// Cargo do servidor que pode usar o comando <c>/moderate</c> do bot (configurado no dashboard).
/// O dono e quem tem permissão de Administrador sempre podem, mesmo sem cargo listado.
/// </summary>
public sealed class ModeratorRole
{
    /// <summary>Limite de cargos por guild (evita listas enormes vindas da API).</summary>
    public const int MaxPerGuild = 25;

    public ulong GuildId { get; set; }
    public Guild Guild { get; set; } = null!;

    public ulong RoleId { get; set; }
}
