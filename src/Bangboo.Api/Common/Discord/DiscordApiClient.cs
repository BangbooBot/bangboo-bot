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
        using var request = BotRequest(HttpMethod.Get, $"applications/{options.Value.ClientId}/commands");
        return await SendAsync<List<DiscordApplicationCommand>>(request, isUserRequest: false, ct);
    }

    private HttpRequestMessage BotRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bot", options.Value.BotToken);
        return request;
    }

    private async Task<T> SendAsync<T>(HttpRequestMessage request, bool isUserRequest, CancellationToken ct)
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
