using NetCord.Services.ApplicationCommands;

public class PingModule : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("ping", "Responde with pong!")]
    public string Ping() => "🏓 pong!";
}