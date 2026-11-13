namespace Bangboo.Api.Dtos;

/// <summary>Estado atual do bot.</summary>
/// <param name="Online">O bot enviou heartbeat recentemente.</param>
/// <param name="GuildCount">Quantidade de servidores em que o bot está.</param>
/// <param name="LatencyMs">Latência do gateway no último heartbeat.</param>
/// <param name="CommandCount">Quantidade de slash commands registrados.</param>
/// <param name="StartedAt">Quando o processo do bot iniciou (null se nunca reportou).</param>
/// <param name="UpdatedAt">Último heartbeat (null se nunca reportou).</param>
public sealed record StatusResponse(
    bool Online,
    int GuildCount,
    int LatencyMs,
    int CommandCount,
    DateTimeOffset? StartedAt,
    DateTimeOffset? UpdatedAt);

/// <summary>Slash command do bot.</summary>
public sealed record CommandResponse(string Name, string Description);
