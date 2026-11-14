namespace Bangboo.Core.Entities;

/// <summary>
/// Canal-armadilha (automod): quem enviar mensagem no canal configurado é punido.
/// Pega contas hackeadas que saem mandando spam em todos os canais. Uma configuração por guild.
/// </summary>
public sealed class Mousetrap
{
    public const int MaxDmMessageLength = 1000;
    public const int MaxMinTenureDays = 365;

    public long Id { get; set; }

    public bool Enabled { get; set; }

    /// <summary>Canal de texto vigiado. Obrigatório quando <see cref="Enabled"/>.</summary>
    public ulong? ChannelId { get; set; }

    public MousetrapAction Action { get; set; } = MousetrapAction.Ban;

    /// <summary>
    /// Só para <see cref="MousetrapAction.ByTenure"/>: membros no servidor há pelo menos
    /// tantos dias são expulsos; os mais novos são banidos.
    /// </summary>
    public int? MinTenureDays { get; set; }

    /// <summary>Envia <see cref="DmMessage"/> ao usuário (por DM) antes de aplicar a punição.</summary>
    public bool SendDm { get; set; }

    /// <summary>Texto da DM. Aceita <c>{user}</c> e <c>{server}</c>.</summary>
    public string? DmMessage { get; set; }

    public ulong GuildId { get; set; }
    public Guild Guild { get; set; } = null!;
}

/// <summary>Punição aplicada pelo mousetrap.</summary>
public enum MousetrapAction
{
    /// <summary>Banimento (apaga as mensagens dos últimos 7 dias).</summary>
    Ban,

    /// <summary>Expulsão com limpeza: bane e desbane na hora (apaga as mensagens; o usuário pode voltar).</summary>
    Kick,

    /// <summary>Pelo tempo no servidor: expulsa quem está há <c>MinTenureDays</c> ou mais; bane os mais novos.</summary>
    ByTenure,
}
