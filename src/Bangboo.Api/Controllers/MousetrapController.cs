using Bangboo.Api.Common.Auth;
using Bangboo.Api.Common.Errors;
using Bangboo.Api.Dtos;
using Bangboo.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bangboo.Api.Controllers;

[ApiController]
[Route("api/guilds/{guildId}/mousetrap")]
[Tags("Mousetrap")]
[Authorize(Policy = AuthPolicies.ManageGuild)]
public sealed class MousetrapController(IMousetrapService mousetrap) : ControllerBase
{
    /// <summary>Configuração do mousetrap.</summary>
    /// <remarks>
    /// Canal-armadilha: quem enviar mensagem nele é punido (pega contas hackeadas que mandam spam).
    /// Se nunca foi configurado, retorna o padrão (desativado).
    /// </remarks>
    /// <param name="guildId">ID da guild no Discord.</param>
    /// <param name="ct">Token de cancelamento.</param>
    [HttpGet]
    [ProducesResponseType<MousetrapResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiError>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiError>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiError>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiError>(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<MousetrapResponse>> Get(string guildId, CancellationToken ct) =>
        Ok(await mousetrap.GetAsync(guildId, ct));

    /// <summary>Salva a configuração do mousetrap.</summary>
    /// <remarks>
    /// Regras: ativar exige um canal de texto do servidor; <c>byTenure</c> exige <c>minTenureDays</c>;
    /// <c>sendDm</c> exige <c>dmMessage</c>. O bot aplica a nova configuração em até 30 s.
    /// </remarks>
    /// <param name="guildId">ID da guild no Discord.</param>
    /// <param name="request">Nova configuração.</param>
    /// <param name="ct">Token de cancelamento.</param>
    [HttpPut]
    [ProducesResponseType<MousetrapResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiError>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiError>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiError>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiError>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiError>(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<MousetrapResponse>> Update(
        string guildId,
        UpdateMousetrapRequest request,
        CancellationToken ct) =>
        Ok(await mousetrap.UpdateAsync(guildId, request, ct));
}
