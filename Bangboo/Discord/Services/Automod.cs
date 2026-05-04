using Bangboo.Data;
using Bangboo.Modules.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NetCord.Gateway;
using NetCord.Rest;

namespace Bangboo.Discord.Services;

public class AutomodService : DiscordServiceModule
{
    public AutomodService(AppDbContext dbContext, IOptions<Env> options) : base(dbContext, options)
    {
    }

    public async Task<bool> Mousetrap(Message message)
    {
        var dbCtx = dbContext;
        
        var gid = message.GuildId;
        var mousetrap = await dbCtx.MousetrapsModel.Where(m => m.FkGuildId == gid).FirstOrDefaultAsync();
        if (mousetrap is null) return false;
        var shouldBan = mousetrap.ShouldBan;
        
        if (message.ChannelId != mousetrap.ChannelId)
        {
            return false;
        }

        var guild = message.Guild;
        var user = message.Author;
        
        var requestProps = new RestRequestProperties();
        requestProps.AuditLogReason = "Free from mousetrap.";
        await guild.BanUserAsync(user.Id, 60 * 60 * 24 * 7, requestProps);
        if (!shouldBan)
            await guild.UnbanUserAsync(user.Id, requestProps);

        if (mousetrap.MessageDm is not null)
        { 
            var dm = await user.GetDMChannelAsync();
            await dm.SendMessageAsync(mousetrap.MessageDm);
        }
        
        return true;
    }
}