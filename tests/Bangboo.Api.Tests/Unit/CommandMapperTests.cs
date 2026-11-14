using Bangboo.Api.Common.Discord;
using Bangboo.Api.Services;

namespace Bangboo.Api.Tests.Unit;

public sealed class CommandMapperTests
{
    private static readonly Dictionary<string, string> Translations = new()
    {
        ["pt-BR"] = "Mostra o perfil",
        ["en-US"] = "Shows the profile (US)",
    };

    [Theory]
    [InlineData("pt-BR", "Mostra o perfil")]
    [InlineData("en", "Shows the profile (US)")] // só o idioma casa com en-US
    [InlineData("de", "Shows the profile")] // sem tradução: padrão
    [InlineData(null, "Shows the profile")]
    public void Localize_PicksTranslationOrFallback(string? locale, string expected) =>
        Assert.Equal(expected, CommandMapper.Localize(Translations, "Shows the profile", locale));

    [Theory]
    [InlineData(1, "subcommand")]
    [InlineData(3, "string")]
    [InlineData(6, "user")]
    [InlineData(99, "unknown")]
    public void OptionTypeName_MapsDiscordTypes(int type, string expected) =>
        Assert.Equal(expected, CommandMapper.OptionTypeName(type));

    [Fact]
    public void ToResponse_MapsOptionsChoicesAndGuildOnly()
    {
        var command = new DiscordApplicationCommand(
            "1", "moderate", "Moderation", Type: 1,
            DescriptionLocalizations: new Dictionary<string, string> { ["pt-BR"] = "Moderação" },
            Contexts: [0],
            Options:
            [
                new DiscordCommandOption(3, "action", "What to do", Required: true,
                    Choices: [new DiscordCommandChoice("ban", new Dictionary<string, string> { ["pt-BR"] = "banir" }), new DiscordCommandChoice("kick")]),
            ]);

        var response = CommandMapper.ToResponse(command, "pt-BR");

        Assert.Equal("Moderação", response.Description);
        Assert.True(response.GuildOnly);
        var option = Assert.Single(response.Options);
        Assert.Equal("string", option.Type);
        Assert.True(option.Required);
        Assert.Equal(["banir", "kick"], option.Choices);
    }

    [Fact]
    public void ToResponse_CommandAvailableInDms_IsNotGuildOnly()
    {
        var everywhere = new DiscordApplicationCommand("1", "social", "Social", Type: 1, Contexts: [0, 1, 2]);
        var unspecified = new DiscordApplicationCommand("2", "fab", "Fab", Type: 1);

        Assert.False(CommandMapper.ToResponse(everywhere, null).GuildOnly);
        Assert.False(CommandMapper.ToResponse(unspecified, null).GuildOnly);
        Assert.Empty(CommandMapper.ToResponse(unspecified, null).Options);
    }
}
