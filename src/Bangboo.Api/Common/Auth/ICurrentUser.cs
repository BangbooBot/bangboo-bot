namespace Bangboo.Api.Common.Auth;

/// <summary>
/// Usuário autenticado da requisição atual. Services dependem desta abstração,
/// nunca de <see cref="HttpContext"/>.
/// </summary>
public interface ICurrentUser
{
    /// <summary>ID do Discord. Lança <c>UnauthorizedException</c> se não houver usuário.</summary>
    string Id { get; }

    string Username { get; }

    string? GlobalName { get; }

    string? AvatarHash { get; }

    /// <summary>Access token OAuth do Discord salvo no cookie.</summary>
    Task<string> GetDiscordAccessTokenAsync();
}
