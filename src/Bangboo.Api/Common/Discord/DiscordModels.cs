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

/// <summary>Comando de <c>GET /applications/{id}/commands</c>.</summary>
public sealed record DiscordApplicationCommand(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("type")] int Type);
