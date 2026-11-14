using Bangboo.Api.Dtos;

namespace Bangboo.Api.Services.Interfaces;

public interface IGuildOverviewService
{
    /// <summary>Visão geral da guild vista pelo bot. <c>NotFoundException</c> se o bot não estiver nela.</summary>
    Task<GuildOverviewResponse> GetOverviewAsync(string guildId, CancellationToken ct);

    /// <summary>Canais de texto/anúncios, na ordem do Discord.</summary>
    Task<IReadOnlyList<GuildChannelResponse>> GetTextChannelsAsync(string guildId, CancellationToken ct);
}
