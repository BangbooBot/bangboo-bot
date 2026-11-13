using Bangboo.Core.Data;
using Bot.Services.BotStatus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Testcontainers.PostgreSql;

namespace Bangboo.Bot.Tests;

public sealed class BotStatusWriterTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:18-alpine")
        .Build();

    private PooledDbContextFactory<AppDbContext> _dbFactory = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync(Ct);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;
        _dbFactory = new PooledDbContextFactory<AppDbContext>(options);

        await using var db = await _dbFactory.CreateDbContextAsync(Ct);
        await db.Database.MigrateAsync(Ct);
    }

    public async ValueTask DisposeAsync() => await _postgres.DisposeAsync();

    [Fact]
    public async Task WriteAsync_FirstHeartbeat_CreatesSingleRow()
    {
        var writer = new BotStatusWriter(_dbFactory);
        var now = DateTimeOffset.UtcNow;

        await writer.WriteAsync(new BotStatusSnapshot(50, 3, now.AddMinutes(-5), now), Ct);

        await using var db = await _dbFactory.CreateDbContextAsync(Ct);
        var status = await db.BotStatus.SingleAsync(Ct);
        Assert.Equal(50, status.LatencyMs);
        Assert.Equal(3, status.GuildCount);
    }

    [Fact]
    public async Task WriteAsync_NextHeartbeats_UpdateSameRow()
    {
        var writer = new BotStatusWriter(_dbFactory);
        var startedAt = DateTimeOffset.UtcNow.AddMinutes(-5);

        await writer.WriteAsync(new BotStatusSnapshot(50, 3, startedAt, startedAt.AddMinutes(1)), Ct);
        await writer.WriteAsync(new BotStatusSnapshot(80, 4, startedAt, startedAt.AddMinutes(2)), Ct);

        await using var db = await _dbFactory.CreateDbContextAsync(Ct);
        var status = await db.BotStatus.SingleAsync(Ct);
        Assert.Equal(80, status.LatencyMs);
        Assert.Equal(4, status.GuildCount);
        Assert.Equal(startedAt.AddMinutes(2), status.UpdatedAt, TimeSpan.FromMilliseconds(1));
    }
}
