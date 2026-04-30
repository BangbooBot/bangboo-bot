using NetCord;
using NetCord.Gateway;
using NetCord.Rest;

namespace Bangboo.Utils.Tools;

public class AutomodService
{
    public Task<bool> Mousetrap(Message message)
    {
        var guild = message.Guild;
        if (guild == null) return Task.FromResult(false);
        
        if (message.ChannelId != 1267667878094835796)
        {
            return Task.FromResult(false);
        }
        
        var member = message.Author as GuildUser;
        if (member == null) return Task.FromResult(false);
        
        /*
        var hasMoreThanThreeDays = DateTimeOffset.UtcNow - member.JoinedAt > TimeSpan.FromDays(3);

        var user = message.Author;
        if (user.IsBot) return Task.FromResult(false);
        
        var requestProps = new RestRequestProperties();
        requestProps.AuditLogReason = "Captured by mousetrap.";
        guild.BanUserAsync(user.Id, 60 * 60 * 24 * 7, requestProps);
        */
        return Task.FromResult(true);
    }
}