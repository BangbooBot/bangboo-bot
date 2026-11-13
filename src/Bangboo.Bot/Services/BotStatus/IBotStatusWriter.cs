namespace Bot.Services.BotStatus;

/// <summary>Foto do estado do bot num instante.</summary>
public sealed record BotStatusSnapshot(int LatencyMs, int GuildCount, DateTimeOffset StartedAt, DateTimeOffset Timestamp);

/// <summary>Persiste o heartbeat do bot (lido pela API em <c>GET api/status</c>).</summary>
public interface IBotStatusWriter
{
    Task WriteAsync(BotStatusSnapshot snapshot, CancellationToken ct);
}
