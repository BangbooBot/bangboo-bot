using Bangboo.Core.Entities;

namespace Bot.Services.Mousetrap;

/// <summary>Punição efetivamente aplicada a quem caiu no mousetrap.</summary>
public enum MousetrapPunishment
{
    /// <summary>Banimento (apaga as mensagens recentes).</summary>
    Ban,

    /// <summary>Expulsão com limpeza: bane e desbane na hora (apaga as mensagens; pode voltar).</summary>
    Kick,
}

/// <summary>Regras puras do mousetrap (sem Discord nem banco), para serem testáveis.</summary>
public static class MousetrapPolicy
{
    /// <summary>
    /// Decide a punição. Em <see cref="MousetrapAction.ByTenure"/>, quem está no servidor há
    /// <paramref name="minTenureDays"/> dias ou mais é expulso; os mais novos (ou com data de entrada
    /// desconhecida) são banidos.
    /// </summary>
    public static MousetrapPunishment Decide(
        MousetrapAction action,
        int? minTenureDays,
        DateTimeOffset? joinedAt,
        DateTimeOffset now) => action switch
    {
        MousetrapAction.Kick => MousetrapPunishment.Kick,
        MousetrapAction.ByTenure when joinedAt is { } joined
                                     && minTenureDays is { } days
                                     && now - joined >= TimeSpan.FromDays(days) => MousetrapPunishment.Kick,
        _ => MousetrapPunishment.Ban,
    };

    /// <summary>Preenche <c>{user}</c> e <c>{server}</c> na mensagem configurada.</summary>
    public static string FormatMessage(string template, string userName, string serverName) =>
        template
            .Replace("{user}", userName, StringComparison.OrdinalIgnoreCase)
            .Replace("{server}", serverName, StringComparison.OrdinalIgnoreCase);
}
