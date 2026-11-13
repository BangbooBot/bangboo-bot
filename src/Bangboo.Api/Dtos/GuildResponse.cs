namespace Bangboo.Api.Dtos;

/// <summary>Guild que o usuário pode gerenciar.</summary>
/// <param name="Id">ID do Discord (string).</param>
/// <param name="Name">Nome da guild.</param>
/// <param name="IconUrl">URL do ícone, se houver.</param>
/// <param name="BannerUrl">URL do banner, se houver.</param>
/// <param name="IsOwner">O usuário é o dono da guild.</param>
/// <param name="IsBotMember">O bot já está na guild (se não, o dashboard oferece o convite).</param>
/// <param name="ApproximateMemberCount">Total aproximado de membros.</param>
/// <param name="ApproximatePresenceCount">Total aproximado de membros online.</param>
public sealed record GuildResponse(
    string Id,
    string Name,
    string? IconUrl,
    string? BannerUrl,
    bool IsOwner,
    bool IsBotMember,
    int ApproximateMemberCount,
    int ApproximatePresenceCount);
