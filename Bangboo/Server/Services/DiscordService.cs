using System.Reflection;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

public class DiscordService
{
    public readonly GatewayClient? Gateway;
    
    public readonly RestClient? Client;

    public readonly List<SlashCommandAttribute?> SlashCommands;
    
    public DiscordService(IHost host)
    {
        Client = host.Services.GetService<RestClient>();
        Gateway = host.Services.GetService<GatewayClient>();
        SlashCommands = Assembly
            .GetExecutingAssembly().GetTypes()
            .SelectMany(type => type.GetMethods())
            .Select(method => method.GetCustomAttribute<SlashCommandAttribute>())
            .Where(command => command is not null)
            .ToList();
    }
}