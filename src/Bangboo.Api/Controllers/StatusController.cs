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
    /// <remarks>
    /// Comandos registrados no Discord, em ordem alfabética, com parâmetros, escolhas e se são só
    /// para servidores. Com <paramref name="locale"/> (ex.: <c>pt-BR</c>), as descrições vêm traduzidas
    /// quando o bot registrou a tradução; senão, em inglês.
    /// </remarks>
    /// <param name="locale">Idioma das descrições (ex.: <c>en</c>, <c>pt-BR</c>).</param>
    /// <param name="ct">Token de cancelamento.</param>
    [HttpGet("commands")]
    [ProducesResponseType<IReadOnlyList<CommandResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiError>(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<IReadOnlyList<CommandResponse>>> Commands(
        [FromQuery] string? locale,
        CancellationToken ct) =>
        Ok(await status.GetCommandsAsync(locale, ct));
}
