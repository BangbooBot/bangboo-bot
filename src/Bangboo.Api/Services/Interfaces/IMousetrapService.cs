using Bangboo.Api.Dtos;

namespace Bangboo.Api.Services.Interfaces;

public interface IMousetrapService
{
    /// <summary>Configuração atual (padrão desativado se a guild nunca configurou).</summary>
    Task<MousetrapResponse> GetAsync(string guildId, CancellationToken ct);

    /// <summary>Valida e salva. Lança <c>ValidationException</c> para combinações inválidas.</summary>
    Task<MousetrapResponse> UpdateAsync(string guildId, UpdateMousetrapRequest request, CancellationToken ct);
}
