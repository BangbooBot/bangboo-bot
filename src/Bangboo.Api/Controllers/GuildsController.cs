using Bangboo.Api.Common.Auth;
using Bangboo.Api.Common.Errors;
using Bangboo.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Bangboo.Api.Services.Interfaces;

namespace Bangboo.Api.Controllers;

[ApiController]
[Route("api/guilds")]
[Tags("Guilds")]
public sealed class GuildsController(IGuildService guilds) : ControllerBase
{
    /// <summary>Guilds gerenciáveis.</summary>
    /// <remarks>
    /// Guilds em que o usuário é dono, Administrator ou tem Manage Server.
    /// <c>isBotMember</c> indica se o bot já está na guild.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<GuildResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiError>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiError>(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<IReadOnlyList<GuildResponse>>> List(CancellationToken ct) =>
        Ok(await guilds.GetManageableGuildsAsync(ct));

    /// <summary>Detalhes de uma guild.</summary>
    /// <remarks>Exige permissão de gerenciar a guild.</remarks>
    /// <param name="guildId">ID da guild no Discord.</param>
    /// <param name="ct">Token de cancelamento.</param>
    [HttpGet("{guildId}")]
    [Authorize(Policy = AuthPolicies.ManageGuild)]
    [ProducesResponseType<GuildResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiError>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiError>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiError>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiError>(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<GuildResponse>> Get(string guildId, CancellationToken ct) =>
        Ok(await guilds.GetGuildAsync(guildId, ct));
}
