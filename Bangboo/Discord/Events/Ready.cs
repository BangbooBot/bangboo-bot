using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace Bangboo.Discord.Events;

public class ReadyGatewayHandler(ILogger<ReadyEventArgs> logger) : IReadyGatewayHandler
{
    public async ValueTask HandleAsync(ReadyEventArgs arg)
    {
        /*
        var guilds = await client.GetCurrentUserGuildsAsync().ToListAsync();
        var guildIds = guilds.Select(g => g.Id);
        var dbCtx = memberService._dbContext;
        //dbCtx.Guilds.Where(g => !guildIds.Contains(g.Id)).ToList().ForEach(g => dbCtx.Guilds.Remove(g));
        var 
        foreach (var guild in guilds)
        {
            
        }
        */
        logger.LogInformation($"● {arg.User.Username} online ✓");
    }
}
