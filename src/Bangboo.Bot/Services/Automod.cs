using Bot.Services.Mousetrap;
using Microsoft.Extensions.Logging;
using NetCord.Gateway;
using NetCord.Rest;

namespace Services;

public interface IAutomodService
{
    Task Mousetrap(Message message);
}

/// <summary>
/// Mousetrap: quem manda mensagem no canal-armadilha configurado no dashboard é punido.
/// Pega contas hackeadas que saem mandando spam em todos os canais.
/// </summary>
public class AutomodService(
    IMousetrapSettingsProvider settingsProvider,
    TimeProvider clock,
    ILogger<AutomodService> logger) : IAutomodService
{
    // Máximo permitido pelo Discord: apaga as mensagens dos últimos 7 dias (limpa o spam).
    private const int DeletedMessageWindowSeconds = 60 * 60 * 24 * 7;

    public async Task Mousetrap(Message message)
    {
        var guild = message.Guild;
        if (guild is null)
        {
            return;
        }

        var settings = await settingsProvider.GetAsync(guild.Id, CancellationToken.None);
        if (settings is not { Enabled: true } || settings.ChannelId != message.ChannelId)
        {
            return;
        }

        var user = message.Author;
        if (user.Id == guild.OwnerId)
        {
            return; // o dono não pode ser punido
        }

        var member = await guild.GetUserAsync(user.Id);
        var punishment = MousetrapPolicy.Decide(settings.Action, settings.MinTenureDays, member.JoinedAt, clock.GetUtcNow());

        // A DM vai ANTES da punição: depois do ban/kick o usuário não divide mais um servidor
        // com o bot e o Discord costuma bloquear a mensagem.
        if (settings.SendDm && !string.IsNullOrWhiteSpace(settings.DmMessage))
        {
            await TrySendDmAsync(user, MousetrapPolicy.FormatMessage(settings.DmMessage, user.Username, guild.Name));
        }

        var requestProperties = new RestRequestProperties
        {
            AuditLogReason = $"Mousetrap: sent a message in the trap channel ({punishment}).",
        };

        try
        {
            await guild.BanUserAsync(user.Id, DeletedMessageWindowSeconds, requestProperties);
            if (punishment == MousetrapPunishment.Kick)
            {
                // "Kick com limpeza": o ban apagou o spam; o unban deixa a pessoa voltar.
                await guild.UnbanUserAsync(user.Id, requestProperties);
            }

            logger.LogInformation(
                "Mousetrap {Punishment} user {UserId} in guild {GuildId}", punishment, user.Id, guild.Id);
        }
        catch (RestException exception)
        {
            // Sem permissão de banir ou o alvo tem cargo acima do bot.
            logger.LogWarning(exception,
                "Mousetrap could not {Punishment} user {UserId} in guild {GuildId}", punishment, user.Id, guild.Id);
        }
    }

    private async Task TrySendDmAsync(NetCord.User user, string content)
    {
        try
        {
            var dm = await user.GetDMChannelAsync();
            await dm.SendMessageAsync(new MessageProperties { Content = content });
        }
        catch (RestException exception)
        {
            // DMs fechadas: segue com a punição mesmo assim.
            logger.LogInformation(exception, "Mousetrap could not DM user {UserId}", user.Id);
        }
    }
}
