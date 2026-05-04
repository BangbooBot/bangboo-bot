using System.Reflection;
using Bangboo.Data;
using Microsoft.Extensions.Options;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace Bangboo.Modules.Services;

public class ServerServicesModule
{
    public readonly GatewayClient Gateway;
    
    public readonly RestClient Client;

    public readonly List<SlashCommandAttribute> SlashCommands;
    
    public readonly AppDbContext _dbContext;
    
    public readonly Env _env;
    
    public ServerServicesModule(IHost host, AppDbContext dbContext, IOptions<Env> options)
    {
        Client = host.Services.GetService<RestClient>();
        Gateway = host.Services.GetService<GatewayClient>();
        SlashCommands = Assembly
            .GetExecutingAssembly().GetTypes()
            .SelectMany(type => type.GetMethods())
            .Select(method => method.GetCustomAttribute<SlashCommandAttribute>())
            .Where(command => command is not null)
            .ToList();
        _dbContext = dbContext;
        _env = options.Value;
    }
}