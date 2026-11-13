using Bangboo.Api.Dtos;

namespace Bangboo.Api.Services.Interfaces;

public interface IStatusService
{
    Task<StatusResponse> GetStatusAsync(CancellationToken ct);

    Task<IReadOnlyList<CommandResponse>> GetCommandsAsync(CancellationToken ct);
}
