namespace Bangboo.Api.Dtos;

/// <summary>Usuário logado no dashboard.</summary>
/// <param name="Id">ID do Discord (string: snowflakes não cabem em number do JS).</param>
/// <param name="Username">Username único do Discord.</param>
/// <param name="GlobalName">Nome de exibição, se definido.</param>
/// <param name="AvatarUrl">URL do avatar (ou o avatar padrão do Discord).</param>
public sealed record UserResponse(string Id, string Username, string? GlobalName, string AvatarUrl);
