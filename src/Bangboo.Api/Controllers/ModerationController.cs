using Bangboo.Api.Common.Auth;
using Bangboo.Api.Common.Errors;
using Bangboo.Api.Dtos;
using Bangboo.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bangboo.Api.Controllers;

[ApiController]
[Route("api/guilds/{guildId}/moderation")]
[Tags("Moderation")]
[Authorize(Policy = AuthPolicies.ManageGuild)]
public sealed class ModerationController(IModerationService moderation) : ControllerBase
{
    /// <summary>Cargos que podem usar o comando /moderate.</summary>
    /// <remarks>O dono e quem tem Administrador sempre podem, mesmo sem cargo listado. 404 se o bot não estiver no servidor.</remarks>
    /// <param name="guildId">ID da guild no Discord.</param>
    /// <param name="ct">Token de cancelamento.</param>
    [HttpGet]
    [ProducesResponseType<ModerationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiError>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiError>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiError>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiError>(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<ModerationResponse>> Get(string guildId, CancellationToken ct) =>
        Ok(await moderation.GetAsync(guildId, ct));

    /// <summary>Define os cargos que podem usar o comando /moderate.</summary>
    /// <remarks>
    /// Substitui a lista atual. Só cargos atribuíveis (sem @everyone e sem cargos de bot), até 25.
    /// O bot aplica a mudança em até 30 s.
    /// </remarks>
    /// <param name="guildId">ID da guild no Discord.</param>
    /// <param name="request">Nova lista de cargos.</param>
    /// <param name="ct">Token de cancelamento.</param>
    [HttpPut]
    [ProducesResponseType<ModerationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiError>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiError>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiError>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiError>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiError>(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<ModerationResponse>> Update(
        string guildId,
        UpdateModerationRequest request,
        CancellationToken ct) =>
        Ok(await moderation.UpdateAsync(guildId, request, ct));
}
