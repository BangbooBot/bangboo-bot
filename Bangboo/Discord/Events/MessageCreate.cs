using Bangboo.Utils.Tools;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace Bangboo.Discord.Events;

public class MessageCreateHandler(ILogger<MessageCreateHandler> logger) : IMessageCreateGatewayHandler
{
    private readonly AutomodService _mousetrapService;
    public ValueTask HandleAsync(Message arg)
    {
        
        return default;
    }
}