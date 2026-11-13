using Bangboo.Api.Dtos;

namespace Bangboo.Api.Services.Interfaces;

public interface IGuildService
{
    /// <summary>Guilds que o usuário atual pode gerenciar; as que já têm o bot vêm primeiro.</summary>
    Task<IReadOnlyList<GuildResponse>> GetManageableGuildsAsync(CancellationToken ct);

    /// <summary>Uma guild gerenciável pelo usuário atual. Lança <c>NotFoundException</c> se não existir.</summary>
    Task<GuildResponse> GetGuildAsync(string guildId, CancellationToken ct);
}
