using System.ComponentModel.DataAnnotations;

namespace Bangboo.Api.Common.Discord;

/// <summary>Seção <c>Discord</c> da configuração. Segredos via user-secrets ou variáveis de ambiente.</summary>
public sealed class DiscordOptions
{
    public const string SectionName = "Discord";

    /// <summary>Application ID (também é o client_id do OAuth).</summary>
    [Required]
    public string ClientId { get; set; } = string.Empty;

    [Required]
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>Token do bot, usado para chamadas REST em nome do bot.</summary>
    [Required]
    public string BotToken { get; set; } = string.Empty;

    [Required, Url]
    public string ApiBaseUrl { get; set; } = "https://discord.com/api/v10/";
}
