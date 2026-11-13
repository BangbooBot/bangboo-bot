using Bangboo.Api.Common.Discord;
using Bangboo.Api.Dtos;
using Bangboo.Core.Data;
using Bangboo.Core.Entities;
using Microsoft.EntityFrameworkCore;

using Bangboo.Api.Services.Interfaces;

namespace Bangboo.Api.Services;

public sealed class StatusService(AppDbContext db, IDiscordApiClient discord, TimeProvider clock) : IStatusService
{
    /// <summary>Sem heartbeat por mais que isso, o bot é considerado offline (o bot envia a cada 30 s).</summary>
    public static readonly TimeSpan OfflineThreshold = TimeSpan.FromSeconds(90);

    // Tipo 1 = CHAT_INPUT (slash command). Os demais são menus de contexto.
    private const int SlashCommandType = 1;

    public async Task<StatusResponse> GetStatusAsync(CancellationToken ct)
    {
        var heartbeat = await db.BotStatus
            .AsNoTracking()
            .FirstOrDefaultAsync(status => status.Id == BotStatus.SingletonId, ct);

        var commands = await GetCommandsAsync(ct);

        if (heartbeat is null)
        {
            return new StatusResponse(false, 0, 0, commands.Count, null, null);
        }

        var online = clock.GetUtcNow() - heartbeat.UpdatedAt <= OfflineThreshold;
        return new StatusResponse(
            online,
            heartbeat.GuildCount,
            heartbeat.LatencyMs,
            commands.Count,
            heartbeat.StartedAt,
            heartbeat.UpdatedAt);
    }

    public async Task<IReadOnlyList<CommandResponse>> GetCommandsAsync(CancellationToken ct)
    {
        var commands = await discord.GetApplicationCommandsAsync(ct);

        return commands
            .Where(command => command.Type == SlashCommandType)
            .OrderBy(command => command.Name, StringComparer.Ordinal)
            .Select(command => new CommandResponse(command.Name, command.Description))
            .ToList();
    }
}
