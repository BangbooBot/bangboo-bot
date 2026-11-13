using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCord.Gateway;

namespace Bot.Services.BotStatus;

/// <summary>
/// Publica latência e quantidade de guilds a cada <see cref="Interval"/>.
/// Falhas de banco são logadas e não derrubam o bot.
/// </summary>
public sealed class BotStatusHeartbeat(
    GatewayClient gateway,
    IBotStatusWriter writer,
    TimeProvider clock,
    ILogger<BotStatusHeartbeat> logger) : BackgroundService
{
    public static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var startedAt = clock.GetUtcNow();
        using var timer = new PeriodicTimer(Interval, clock);

        do
        {
            await PublishAsync(startedAt, stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task PublishAsync(DateTimeOffset startedAt, CancellationToken ct)
    {
        try
        {
            var snapshot = new BotStatusSnapshot(
                LatencyMs: (int)gateway.Latency.TotalMilliseconds,
                GuildCount: gateway.Cache.Guilds.Count,
                StartedAt: startedAt,
                Timestamp: clock.GetUtcNow());

            await writer.WriteAsync(snapshot, ct);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogWarning(exception, "Failed to publish bot status heartbeat");
        }
    }
}
