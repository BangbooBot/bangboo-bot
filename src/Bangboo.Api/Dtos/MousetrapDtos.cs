using System.ComponentModel.DataAnnotations;
using Bangboo.Core.Entities;

namespace Bangboo.Api.Dtos;

/// <summary>Configuração do mousetrap (canal-armadilha) de um servidor.</summary>
/// <param name="Enabled">Ativo.</param>
/// <param name="ChannelId">Canal vigiado (null se ainda não configurado).</param>
/// <param name="ChannelName">Nome do canal, se ele ainda existir.</param>
/// <param name="Action">Punição: <c>ban</c>, <c>kick</c> (bane e desbane, apagando as mensagens) ou <c>byTenure</c>.</param>
/// <param name="MinTenureDays">Para <c>byTenure</c>: dias no servidor a partir dos quais o membro é expulso em vez de banido.</param>
/// <param name="SendDm">Envia uma mensagem ao usuário antes da punição.</param>
/// <param name="DmMessage">Texto da mensagem; aceita <c>{user}</c> e <c>{server}</c>.</param>
public sealed record MousetrapResponse(
    bool Enabled,
    string? ChannelId,
    string? ChannelName,
    MousetrapAction Action,
    int? MinTenureDays,
    bool SendDm,
    string? DmMessage);

/// <summary>Atualiza a configuração do mousetrap.</summary>
public sealed record UpdateMousetrapRequest
{
    /// <summary>Ativo. Exige <see cref="ChannelId"/>.</summary>
    public bool Enabled { get; init; }

    /// <summary>Canal de texto vigiado.</summary>
    public string? ChannelId { get; init; }

    /// <summary>Punição aplicada.</summary>
    public MousetrapAction Action { get; init; } = MousetrapAction.Ban;

    /// <summary>Obrigatório para <c>byTenure</c>.</summary>
    [Range(1, Mousetrap.MaxMinTenureDays, ErrorMessage = "Minimum tenure must be between 1 and 365 days")]
    public int? MinTenureDays { get; init; }

    /// <summary>Envia <see cref="DmMessage"/> antes da punição.</summary>
    public bool SendDm { get; init; }

    /// <summary>Obrigatório quando <see cref="SendDm"/>. Aceita <c>{user}</c> e <c>{server}</c>.</summary>
    [MaxLength(Mousetrap.MaxDmMessageLength, ErrorMessage = "The message must be at most 1000 characters")]
    public string? DmMessage { get; init; }
}
