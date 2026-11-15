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
public sealed class GuildsController(IGuildService guilds, IGuildOverviewService overview) : ControllerBase
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

    /// <summary>Visão geral do servidor.</summary>
    /// <remarks>
    /// Membros, online, canais, cargos, impulsos e configurações do Bangboo. Exige permissão de
    /// gerenciar a guild; 404 se o bot não estiver no servidor.
    /// </remarks>
    /// <param name="guildId">ID da guild no Discord.</param>
    /// <param name="ct">Token de cancelamento.</param>
    [HttpGet("{guildId}/overview")]
    [Authorize(Policy = AuthPolicies.ManageGuild)]
    [ProducesResponseType<GuildOverviewResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiError>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiError>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiError>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiError>(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<GuildOverviewResponse>> Overview(string guildId, CancellationToken ct) =>
        Ok(await overview.GetOverviewAsync(guildId, ct));

    /// <summary>Canais de texto do servidor.</summary>
    /// <remarks>Canais de texto e anúncios, na ordem do Discord (para seletores). 404 se o bot não estiver no servidor.</remarks>
    /// <param name="guildId">ID da guild no Discord.</param>
    /// <param name="ct">Token de cancelamento.</param>
    [HttpGet("{guildId}/channels")]
    [Authorize(Policy = AuthPolicies.ManageGuild)]
    [ProducesResponseType<IReadOnlyList<GuildChannelResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiError>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiError>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiError>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiError>(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<IReadOnlyList<GuildChannelResponse>>> Channels(string guildId, CancellationToken ct) =>
        Ok(await overview.GetTextChannelsAsync(guildId, ct));

    /// <summary>Cargos do servidor.</summary>
    /// <remarks>Cargos que podem ser dados a membros (sem @everyone e sem cargos de bot), do mais alto para o mais baixo. 404 se o bot não estiver no servidor.</remarks>
    /// <param name="guildId">ID da guild no Discord.</param>
    /// <param name="ct">Token de cancelamento.</param>
    [HttpGet("{guildId}/roles")]
    [Authorize(Policy = AuthPolicies.ManageGuild)]
    [ProducesResponseType<IReadOnlyList<GuildRoleResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiError>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiError>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiError>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiError>(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<IReadOnlyList<GuildRoleResponse>>> Roles(string guildId, CancellationToken ct) =>
        Ok(await overview.GetRolesAsync(guildId, ct));
}
