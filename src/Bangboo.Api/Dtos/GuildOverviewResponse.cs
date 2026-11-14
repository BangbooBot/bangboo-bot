namespace Bangboo.Api.Dtos;

/// <summary>Visão geral de um servidor (dados do Discord + configurações do Bangboo).</summary>
/// <param name="Id">ID da guild.</param>
/// <param name="Name">Nome.</param>
/// <param name="IconUrl">Ícone, se houver.</param>
/// <param name="BannerUrl">Banner, se houver.</param>
/// <param name="Description">Descrição pública (servidores da comunidade).</param>
/// <param name="OwnerId">ID do dono.</param>
/// <param name="CreatedAt">Data de criação (derivada do ID).</param>
/// <param name="MemberCount">Total aproximado de membros.</param>
/// <param name="OnlineCount">Total aproximado de membros online.</param>
/// <param name="BoostTier">Nível de impulso (0–3).</param>
/// <param name="BoostCount">Quantidade de impulsos.</param>
/// <param name="TextChannelCount">Canais de texto e anúncios.</param>
/// <param name="VoiceChannelCount">Canais de voz e palco.</param>
/// <param name="CategoryCount">Categorias.</param>
/// <param name="RoleCount">Cargos (sem contar @everyone).</param>
/// <param name="PreferredLocale">Idioma principal do servidor (ex.: <c>pt-BR</c>).</param>
/// <param name="Mousetrap">Configuração atual do mousetrap.</param>
public sealed record GuildOverviewResponse(
    string Id,
    string Name,
    string? IconUrl,
    string? BannerUrl,
    string? Description,
    string OwnerId,
    DateTimeOffset CreatedAt,
    int MemberCount,
    int OnlineCount,
    int BoostTier,
    int BoostCount,
    int TextChannelCount,
    int VoiceChannelCount,
    int CategoryCount,
    int RoleCount,
    string? PreferredLocale,
    MousetrapResponse Mousetrap);

/// <summary>Canal de texto da guild (para seletores do dashboard).</summary>
/// <param name="Id">ID do canal.</param>
/// <param name="Name">Nome (sem #).</param>
/// <param name="Category">Categoria em que está, se houver.</param>
public sealed record GuildChannelResponse(string Id, string Name, string? Category);
