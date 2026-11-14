using Bangboo.Api.Dtos;

namespace Bangboo.Api.Services.Interfaces;

public interface IStatusService
{
    Task<StatusResponse> GetStatusAsync(CancellationToken ct);

    /// <summary>Slash commands do bot, com descrições no idioma <paramref name="locale"/> quando houver tradução.</summary>
    Task<IReadOnlyList<CommandResponse>> GetCommandsAsync(string? locale, CancellationToken ct);
}
