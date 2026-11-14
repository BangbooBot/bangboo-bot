using System.Net;
using System.Net.Http.Headers;
using Bangboo.Api.Common.Errors;
using Microsoft.Extensions.Options;

namespace Bangboo.Api.Common.Discord;

/// <summary>Implementação HTTP de <see cref="IDiscordApiClient"/> (typed HttpClient).</summary>
public sealed class DiscordApiClient(
    HttpClient http,
    IOptions<DiscordOptions> options,
    ILogger<DiscordApiClient> logger) : IDiscordApiClient
{
    // Limite máximo de itens por página em /users/@me/guilds.
    private const int GuildPageSize = 200;

    public async Task<IReadOnlyList<DiscordPartialGuild>> GetUserGuildsAsync(string userAccessToken, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "users/@me/guilds?with_counts=true");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccessToken);

        return await SendAsync<List<DiscordPartialGuild>>(request, isUserRequest: true, ct);
    }

    public async Task<IReadOnlySet<string>> GetBotGuildIdsAsync(CancellationToken ct)
    {
        var ids = new HashSet<string>();
        string? after = null;

        while (true)
        {
            var url = $"users/@me/guilds?limit={GuildPageSize}" + (after is null ? "" : $"&after={after}");
            using var request = BotRequest(HttpMethod.Get, url);
            var page = await SendAsync<List<DiscordPartialGuild>>(request, isUserRequest: false, ct);

            foreach (var guild in page)
            {
                ids.Add(guild.Id);
            }

            if (page.Count < GuildPageSize)
            {
                return ids;
            }

            after = page[^1].Id;
        }
    }

    public async Task<IReadOnlyList<DiscordApplicationCommand>> GetApplicationCommandsAsync(CancellationToken ct)
    {
        using var request = BotRequest(HttpMethod.Get, $"applications/{options.Value.ClientId}/commands?with_localizations=true");
        return await SendAsync<List<DiscordApplicationCommand>>(request, isUserRequest: false, ct);
    }

    public async Task<DiscordGuild> GetGuildAsync(string guildId, CancellationToken ct)
    {
        using var request = BotRequest(HttpMethod.Get, $"guilds/{Uri.EscapeDataString(guildId)}?with_counts=true");
        return await SendAsync<DiscordGuild>(request, isUserRequest: false, ct, guildScoped: true);
    }

    public async Task<IReadOnlyList<DiscordChannel>> GetGuildChannelsAsync(string guildId, CancellationToken ct)
    {
        using var request = BotRequest(HttpMethod.Get, $"guilds/{Uri.EscapeDataString(guildId)}/channels");
        return await SendAsync<List<DiscordChannel>>(request, isUserRequest: false, ct, guildScoped: true);
    }

    private HttpRequestMessage BotRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bot", options.Value.BotToken);
        return request;
    }

    /// <param name="guildScoped">
    /// Rota de uma guild específica com o token do bot: 403/404 significam que o bot não está
    /// (ou não tem acesso) na guild, e viram 404 para o dashboard oferecer o convite.
    /// </param>
    private async Task<T> SendAsync<T>(HttpRequestMessage request, bool isUserRequest, CancellationToken ct, bool guildScoped = false)
    {
        HttpResponseMessage response;
        try
        {
            response = await http.SendAsync(request, ct);
        }
        catch (HttpRequestException exception)
        {
            logger.LogError(exception, "Discord request failed: {Url}", request.RequestUri);
            throw new ExternalServiceException("Discord is unavailable, try again later");
        }

        using (response)
        {
            // Token OAuth do usuário expirado/revogado: o front deve refazer o login.
            if (isUserRequest && response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException("Discord session expired, please log in again");
            }

            if (guildScoped && response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Forbidden)
            {
                throw new NotFoundException("Bangboo is not in this server");
            }

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Discord returned {Status} for {Url}", (int)response.StatusCode, request.RequestUri);
                throw new ExternalServiceException("Discord request failed, try again later");
            }

            return await response.Content.ReadFromJsonAsync<T>(ct)
                ?? throw new ExternalServiceException("Discord returned an empty response");
        }
    }
}
