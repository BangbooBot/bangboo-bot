using System.Reflection;
using Bot.Services.Localization;
using NetCord.Services.ApplicationCommands;

namespace Bangboo.Bot.Tests;

/// <summary>
/// Garante que as traduções em <c>Localizations/pt-BR.json</c> são lidas pelo NetCord
/// e que nenhum slash command fica sem descrição em português.
/// </summary>
public sealed class CommandLocalizationsTests
{
    private const string Locale = "pt-BR";

    private readonly ILocalizationsProvider _provider = CommandLocalizations.CreateProvider();

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private async Task<string?> TranslateAsync(params LocalizationPathSegment[] path)
    {
        var translations = await _provider.GetLocalizationsAsync(path, Ct);
        return translations is not null && translations.TryGetValue(Locale, out var value) ? value : null;
    }

    [Fact]
    public async Task CommandDescription_IsTranslated()
    {
        var description = await TranslateAsync(
            new ApplicationCommandLocalizationPathSegment("profile"),
            DescriptionLocalizationPathSegment.Instance);

        Assert.Equal("Mostra as suas informações ou as de outro membro do servidor", description);
    }

    [Fact]
    public async Task ParameterDescription_IsTranslated()
    {
        var description = await TranslateAsync(
            new ApplicationCommandLocalizationPathSegment("profile"),
            new SlashCommandParameterLocalizationPathSegment("user"),
            DescriptionLocalizationPathSegment.Instance);

        Assert.Equal("Usuário selecionado", description);
    }

    [Fact]
    public async Task ChoiceNames_AreTranslated()
    {
        var field = typeof(ModAction).GetField(nameof(ModAction.Ban))!;

        // Caminho que o NetCord usa para escolhas de enum: enum → campo → nome.
        var name = await TranslateAsync(
            new EnumLocalizationPathSegment(typeof(ModAction)),
            new EnumFieldLocalizationPathSegment(field),
            NameLocalizationPathSegment.Instance);

        Assert.Equal("banir", name);
    }

    [Fact]
    public async Task EverySlashCommand_HasPortugueseDescription()
    {
        var commandNames = typeof(ModAction).Assembly.GetTypes()
            .SelectMany(type => type.GetMethods())
            .Select(method => method.GetCustomAttribute<SlashCommandAttribute>())
            .OfType<SlashCommandAttribute>()
            .Select(attribute => attribute.Name)
            .ToList();

        Assert.NotEmpty(commandNames);
        foreach (var name in commandNames)
        {
            var description = await TranslateAsync(
                new ApplicationCommandLocalizationPathSegment(name),
                DescriptionLocalizationPathSegment.Instance);

            Assert.False(string.IsNullOrWhiteSpace(description), $"/{name} não tem descrição em {Locale} em Localizations/{Locale}.json");
        }
    }
}
