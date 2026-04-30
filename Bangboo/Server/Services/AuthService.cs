using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Bangboo.Server.Services;

public class AuthService
{
    private readonly Env _env;

    public AuthService(IOptions<Env> ptions)
    {
        _env = ptions.Value;
    }

    public Task<HttpResponseMessage> Authorize()
    {
        var clientId = _env.ClientId;
        var redirectUri = _env.RedirectUrl;

        var query = new Dictionary<string, string?>
        {
            ["client_id"] = $"{clientId}",
            ["response_type"] = "code",
            ["redirect_uri"] = redirectUri,
            ["scope"] = "identify guilds email"
        };
        
        var uri = QueryHelpers.AddQueryString(
            $"{_env.DiscordApiUrl}/oauth2/authorize",
            query
        );

        var client = new HttpClient();

        return client.GetAsync(uri);
    }

    public Task<HttpResponseMessage> TokenExchange(String code)
    {
        var clientId = _env.ClientId;
        var clientSecret = _env.ClientSecret;
        var redirectUri = _env.RedirectUrl;

        var body = new Dictionary<string, string>
        {
            ["client_id"] = $"{clientId}",
            ["client_secret"] = clientSecret,
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = redirectUri,
            ["Content-Type"] = "application/x-www-form-urlencoded"
        };

        var content = new FormUrlEncodedContent(body);

        var client = new HttpClient();

        return client.PostAsync(
            $"{_env.DiscordApiUrl}/oauth2/token",
            content
        );
    }

    public Task<HttpResponseMessage> RefreshToken(String refreshToken)
    {
        var clientId = _env.ClientId;
        var clientSecret = _env.ClientSecret;
        var redirectUri = _env.RedirectUrl;

        var body = new Dictionary<string, string>
        {
            ["client_id"] = $"{clientId}",
            ["client_secret"] = clientSecret,
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken,
            ["redirect_uri"] = redirectUri
        };
        
        var content = new FormUrlEncodedContent(body);

        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");

        return client.PostAsync(
            $"{_env.DiscordApiUrl}/oauth2/token",
            content
        );
    }
    
    public Task<HttpResponseMessage> GetUserInfo(String token)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client.GetAsync(
            $"{_env.DiscordApiUrl}/users/@me"
        );
    }
}