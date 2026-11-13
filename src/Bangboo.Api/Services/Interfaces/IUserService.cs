using Bangboo.Api.Dtos;

namespace Bangboo.Api.Services.Interfaces;

public interface IUserService
{
    /// <summary>Cria ou atualiza o usuário a partir do perfil do Discord (a cada login).</summary>
    Task UpsertAsync(DiscordProfile profile, CancellationToken ct);
}
