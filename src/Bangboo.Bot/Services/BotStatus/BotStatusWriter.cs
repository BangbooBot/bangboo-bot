using Bangboo.Core.Data;
using Microsoft.EntityFrameworkCore;
using BotStatusEntity = Bangboo.Core.Entities.BotStatus;

namespace Bot.Services.BotStatus;

public sealed class BotStatusWriter(IDbContextFactory<AppDbContext> dbFactory) : IBotStatusWriter
{
    public async Task WriteAsync(BotStatusSnapshot snapshot, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);

        var status = await db.BotStatus.FirstOrDefaultAsync(s => s.Id == BotStatusEntity.SingletonId, ct);
        if (status is null)
        {
            status = new BotStatusEntity();
            db.BotStatus.Add(status);
        }

        status.LatencyMs = snapshot.LatencyMs;
        status.GuildCount = snapshot.GuildCount;
        status.StartedAt = snapshot.StartedAt;
        status.UpdatedAt = snapshot.Timestamp;

        await db.SaveChangesAsync(ct);
    }
}
