using System.Text.Json.Serialization;

namespace Bangboo.Api.Common.Discord;

// Modelos de resposta da API REST do Discord (apenas os campos usados).
// Não são DTOs da nossa API: nunca retorne estes tipos em um controller.

/// <summary>Guild parcial de <c>GET /users/@me/guilds</c>.</summary>
public sealed record DiscordPartialGuild(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("icon")] string? Icon,
    [property: JsonPropertyName("banner")] string? Banner,
    [property: JsonPropertyName("owner")] bool Owner,
    [property: JsonPropertyName("permissions")] string? Permissions,
    [property: JsonPropertyName("approximate_member_count")] int? ApproximateMemberCount,
    [property: JsonPropertyName("approximate_presence_count")] int? ApproximatePresenceCount);

/// <summary>
/// Comando de <c>GET /applications/{id}/commands?with_localizations=true</c>.
/// <c>Type</c>: 1 = slash, 2 = menu de usuário, 3 = menu de mensagem.
/// </summary>
public sealed record DiscordApplicationCommand(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("type")] int Type,
    [property: JsonPropertyName("description_localizations")] IReadOnlyDictionary<string, string>? DescriptionLocalizations = null,
    /// <summary>Onde o comando pode ser usado: 0 = servidor, 1 = DM com o bot, 2 = DMs/grupos privados.</summary>
    [property: JsonPropertyName("contexts")] IReadOnlyList<int>? Contexts = null,
    [property: JsonPropertyName("options")] IReadOnlyList<DiscordCommandOption>? Options = null);

/// <summary>
/// Parâmetro (ou subcomando) de um comando.
/// <c>Type</c>: 1 = subcomando, 2 = grupo, 3 = string, 4 = inteiro, 5 = booleano, 6 = usuário,
/// 7 = canal, 8 = cargo, 9 = menção, 10 = número, 11 = anexo.
/// </summary>
public sealed record DiscordCommandOption(
    [property: JsonPropertyName("type")] int Type,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("description_localizations")] IReadOnlyDictionary<string, string>? DescriptionLocalizations = null,
    [property: JsonPropertyName("required")] bool Required = false,
    [property: JsonPropertyName("choices")] IReadOnlyList<DiscordCommandChoice>? Choices = null,
    [property: JsonPropertyName("options")] IReadOnlyList<DiscordCommandOption>? Options = null);

/// <summary>Guild completa de <c>GET /guilds/{id}?with_counts=true</c> (com o token do bot).</summary>
public sealed record DiscordGuild(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("icon")] string? Icon,
    [property: JsonPropertyName("banner")] string? Banner,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("owner_id")] string OwnerId,
    [property: JsonPropertyName("premium_tier")] int PremiumTier,
    [property: JsonPropertyName("premium_subscription_count")] int? PremiumSubscriptionCount,
    [property: JsonPropertyName("approximate_member_count")] int? ApproximateMemberCount,
    [property: JsonPropertyName("approximate_presence_count")] int? ApproximatePresenceCount,
    [property: JsonPropertyName("preferred_locale")] string? PreferredLocale,
    [property: JsonPropertyName("roles")] IReadOnlyList<DiscordRole>? Roles = null);

/// <summary>Cargo da guild. <c>Managed</c> = cargo de bot/integração (não pode ser dado a membros).</summary>
public sealed record DiscordRole(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("color")] int Color = 0,
    [property: JsonPropertyName("position")] int Position = 0,
    [property: JsonPropertyName("managed")] bool Managed = false);

/// <summary>
/// Canal de <c>GET /guilds/{id}/channels</c>.
/// <c>Type</c>: 0 = texto, 2 = voz, 4 = categoria, 5 = anúncios, 13 = palco, 15 = fórum, 16 = mídia.
/// </summary>
public sealed record DiscordChannel(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("type")] int Type,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("position")] int Position,
    [property: JsonPropertyName("parent_id")] string? ParentId);

/// <summary>Escolha fixa de um parâmetro (o valor em si não é exibido).</summary>
public sealed record DiscordCommandChoice(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("name_localizations")] IReadOnlyDictionary<string, string>? NameLocalizations = null);
