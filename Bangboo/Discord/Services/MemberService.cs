using Bangboo.Data;
using Bangboo.Modules.Services;
using Microsoft.Extensions.Options;

namespace Bangboo.Discord.Services;

public class MemberService : DiscordServiceModule
{
    public MemberService(AppDbContext dbContext, IOptions<Env> options) : base(dbContext, options)
    {
    }
    
    
}