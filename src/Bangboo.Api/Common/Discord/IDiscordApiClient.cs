namespace Bangboo.Api.Common.Discord;

/// <summary>Acesso à API REST do Discord. Falhas viram <c>AppException</c>.</summary>
public interface IDiscordApiClient
{
    /// <summary>Guilds do usuário dono do <paramref name="userAccessToken"/> (OAuth, scope <c>guilds</c>).</summary>
    Task<IReadOnlyList<DiscordPartialGuild>> GetUserGuildsAsync(string userAccessToken, CancellationToken ct);

    /// <summary>IDs das guilds em que o bot está.</summary>
    Task<IReadOnlySet<string>> GetBotGuildIdsAsync(CancellationToken ct);

    /// <summary>Comandos globais registrados pela aplicação.</summary>
    Task<IReadOnlyList<DiscordApplicationCommand>> GetApplicationCommandsAsync(CancellationToken ct);
}
