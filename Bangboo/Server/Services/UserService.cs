using System.Net.Http.Headers;
using Bangboo.Data;
using Bangboo.Modules.Services;
using Microsoft.Extensions.Options;

namespace Bangboo.Server.Services;

public class UserService : ServerServicesModule
{
    public UserService(IHost host, AppDbContext dbContext, IOptions<Env> options) : base(host, dbContext, options)
    {
    }

    public Task<HttpResponseMessage> GetUserInfo(String token)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client.GetAsync(
            $"{_env.DiscordApiUrl}/users/@me"
        );
    }
    
    public Task<HttpResponseMessage> GetUserGuilds(String token)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client.GetAsync(
            $"{_env.DiscordApiUrl}/users/@me/guilds"
        );
    }
}