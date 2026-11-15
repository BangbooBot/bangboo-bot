using Bangboo.Api.Dtos;

namespace Bangboo.Api.Services.Interfaces;

public interface IGuildOverviewService
{
    /// <summary>Visão geral da guild vista pelo bot. <c>NotFoundException</c> se o bot não estiver nela.</summary>
    Task<GuildOverviewResponse> GetOverviewAsync(string guildId, CancellationToken ct);

    /// <summary>Canais de texto/anúncios, na ordem do Discord.</summary>
    Task<IReadOnlyList<GuildChannelResponse>> GetTextChannelsAsync(string guildId, CancellationToken ct);

    /// <summary>Cargos atribuíveis (sem @everyone e cargos de bot), do mais alto para o mais baixo.</summary>
    Task<IReadOnlyList<GuildRoleResponse>> GetRolesAsync(string guildId, CancellationToken ct);
}
