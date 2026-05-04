using Bangboo.Data;
using Microsoft.Extensions.Options;

namespace Bangboo.Modules.Services;

public class DiscordServiceModule
{
    public readonly AppDbContext dbContext;
    
    public readonly Env env;
    
    public DiscordServiceModule(AppDbContext dbContext, IOptions<Env> options)
    {
        this.dbContext = dbContext;
        this.env = options.Value;
    }
}