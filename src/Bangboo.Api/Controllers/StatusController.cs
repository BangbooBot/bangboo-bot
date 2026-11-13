using Bangboo.Api.Common.Errors;
using Bangboo.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Bangboo.Api.Services.Interfaces;

namespace Bangboo.Api.Controllers;

[ApiController]
[Route("api/status")]
[Tags("Status")]
[AllowAnonymous]
public sealed class StatusController(IStatusService status) : ControllerBase
{
    /// <summary>Status do bot.</summary>
    /// <remarks>Online/offline (pelo heartbeat do bot), servidores, latência e quantidade de comandos.</remarks>
    [HttpGet]
    [ProducesResponseType<StatusResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiError>(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<StatusResponse>> Get(CancellationToken ct) =>
        Ok(await status.GetStatusAsync(ct));

    /// <summary>Slash commands do bot.</summary>
    /// <remarks>Lista de comandos registrados no Discord, em ordem alfabética.</remarks>
    [HttpGet("commands")]
    [ProducesResponseType<IReadOnlyList<CommandResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiError>(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<IReadOnlyList<CommandResponse>>> Commands(CancellationToken ct) =>
        Ok(await status.GetCommandsAsync(ct));
}
