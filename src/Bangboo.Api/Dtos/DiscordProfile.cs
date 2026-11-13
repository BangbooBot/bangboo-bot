namespace Bangboo.Api.Dtos;

/// <summary>Perfil do Discord recebido no login (entrada interna, não exposta na API).</summary>
public sealed record DiscordProfile(string Id, string Username, string? GlobalName, string? AvatarHash);
