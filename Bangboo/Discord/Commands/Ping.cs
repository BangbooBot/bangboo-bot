using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

public class PingModule : ApplicationCommandModule<ApplicationCommandContext>
{
    private readonly Emojis _emojis;
    
    public PingModule(Emojis emojis)
    {
        _emojis = emojis;
    }
    
    [SlashCommand("ping", "Responde with pong!")]
    public async Task Ping()
    {
        await Context.Interaction.SendResponseAsync(
            InteractionCallback.DeferredMessage()
        );
        
        await Context.Interaction.ModifyResponseAsync(message =>
            message.Content = $"🏓 pong! {_emojis.Static.ActionX}"
        );
    }
}