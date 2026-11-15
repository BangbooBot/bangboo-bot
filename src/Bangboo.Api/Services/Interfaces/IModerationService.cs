using Bangboo.Api.Dtos;

namespace Bangboo.Api.Services.Interfaces;

public interface IModerationService
{
    /// <summary>Cargos que podem usar o <c>/moderate</c> (lista vazia se nunca configurado).</summary>
    Task<ModerationResponse> GetAsync(string guildId, CancellationToken ct);

    /// <summary>Substitui os cargos. <c>ValidationException</c> se algum não for cargo atribuível da guild.</summary>
    Task<ModerationResponse> UpdateAsync(string guildId, UpdateModerationRequest request, CancellationToken ct);
}
