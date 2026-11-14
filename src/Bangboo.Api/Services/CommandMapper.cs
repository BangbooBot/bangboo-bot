using Bangboo.Api.Common.Discord;
using Bangboo.Api.Dtos;

namespace Bangboo.Api.Services;

/// <summary>Converte comandos da API do Discord para os DTOs da nossa API, no idioma pedido.</summary>
public static class CommandMapper
{
    // Contexto 0 = dentro de servidores (Discord InteractionContextType.GUILD).
    private const int GuildContext = 0;

    public static CommandResponse ToResponse(DiscordApplicationCommand command, string? locale) => new(
        command.Name,
        Localize(command.DescriptionLocalizations, command.Description, locale),
        IsGuildOnly(command.Contexts),
        MapOptions(command.Options, locale));

    /// <summary>
    /// Tradução do Discord para <paramref name="locale"/> (ex.: "pt-BR"). Aceita também só o idioma
    /// ("en" casa com "en-US"). Sem tradução, devolve <paramref name="fallback"/> (a descrição padrão).
    /// </summary>
    public static string Localize(IReadOnlyDictionary<string, string>? translations, string fallback, string? locale)
    {
        if (translations is null || string.IsNullOrWhiteSpace(locale))
        {
            return fallback;
        }

        if (translations.TryGetValue(locale, out var exact) && !string.IsNullOrWhiteSpace(exact))
        {
            return exact;
        }

        var language = locale.Split('-')[0];
        var sameLanguage = translations
            .Where(entry => entry.Key.Split('-')[0].Equals(language, StringComparison.OrdinalIgnoreCase))
            .Select(entry => entry.Value)
            .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

        return sameLanguage ?? fallback;
    }

    /// <summary>Nome legível do tipo de opção do Discord.</summary>
    public static string OptionTypeName(int type) => type switch
    {
        1 => "subcommand",
        2 => "subcommand-group",
        3 => "string",
        4 => "integer",
        5 => "boolean",
        6 => "user",
        7 => "channel",
        8 => "role",
        9 => "mentionable",
        10 => "number",
        11 => "attachment",
        _ => "unknown",
    };

    /// <summary>Só servidores quando os contextos permitidos são exatamente "servidor".</summary>
    private static bool IsGuildOnly(IReadOnlyList<int>? contexts) =>
        contexts is { Count: > 0 } && contexts.All(context => context == GuildContext);

    private static IReadOnlyList<CommandOptionResponse> MapOptions(
        IReadOnlyList<DiscordCommandOption>? options,
        string? locale) =>
        options is null
            ? []
            : options.Select(option => new CommandOptionResponse(
                    option.Name,
                    Localize(option.DescriptionLocalizations, option.Description, locale),
                    OptionTypeName(option.Type),
                    option.Required,
                    option.Choices?.Select(choice => Localize(choice.NameLocalizations, choice.Name, locale)).ToList() ?? [],
                    MapOptions(option.Options, locale)))
                .ToList();
}
