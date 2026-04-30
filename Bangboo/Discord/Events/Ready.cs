using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace Bangboo.Discord.Events;

public class ReadyGatewayHandler(ILogger<ReadyEventArgs> logger) : IReadyGatewayHandler
{
    public ValueTask HandleAsync(ReadyEventArgs arg)
    {
        logger.LogInformation($"● {arg.User.Username} online ✓");
        return default;
    }
}
