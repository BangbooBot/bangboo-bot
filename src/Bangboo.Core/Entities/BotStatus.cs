namespace Bangboo.Core.Entities;

/// <summary>
/// Heartbeat publicado pelo bot. Existe uma única linha (<see cref="SingletonId"/>),
/// atualizada periodicamente; a API lê daqui para o endpoint de status.
/// </summary>
public sealed class BotStatus
{
    public const int SingletonId = 1;

    public int Id { get; set; } = SingletonId;
    public int LatencyMs { get; set; }
    public int GuildCount { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
