using System.Net.Http.Json;
using Bangboo.Api.Common.Discord;
using Bangboo.Api.Dtos;
using Bangboo.Core.Data;
using Bangboo.Core.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Bangboo.Api.Tests.Integration;

public sealed class StatusEndpointsTests(BangbooApiFactory factory) : ApiTestBase(factory)
{
    [Fact]
    public async Task Status_WithoutHeartbeat_IsOffline()
    {
        GivenCommands("ping");

        var status = await Factory.CreateClientAs(null).GetFromJsonAsync<StatusResponse>("/api/status", Ct);

        Assert.NotNull(status);
        Assert.False(status.Online);
        Assert.Equal(1, status.CommandCount);
        Assert.Null(status.UpdatedAt);
    }

    [Fact]
    public async Task Status_WithRecentHeartbeat_IsOnline()
    {
        GivenCommands();
        await GivenHeartbeatAsync(updatedAt: DateTimeOffset.UtcNow, guildCount: 7);

        var status = await Factory.CreateClientAs(null).GetFromJsonAsync<StatusResponse>("/api/status", Ct);

        Assert.NotNull(status);
        Assert.True(status.Online);
        Assert.Equal(7, status.GuildCount);
    }

    [Fact]
    public async Task Status_WithStaleHeartbeat_IsOffline()
    {
        GivenCommands();
        await GivenHeartbeatAsync(updatedAt: DateTimeOffset.UtcNow.AddMinutes(-10), guildCount: 7);

        var status = await Factory.CreateClientAs(null).GetFromJsonAsync<StatusResponse>("/api/status", Ct);

        Assert.NotNull(status);
        Assert.False(status.Online);
    }

    [Fact]
    public async Task Commands_ReturnsOnlySlashCommandsSorted()
    {
        Factory.Discord.GetApplicationCommandsAsync(Arg.Any<CancellationToken>()).Returns(
        [
            new DiscordApplicationCommand("1", "profile", "Profile", Type: 1),
            new DiscordApplicationCommand("2", "Report message", "", Type: 3),
            new DiscordApplicationCommand("3", "fab", "Fab", Type: 1),
        ]);

        var commands = await Factory.CreateClientAs(null)
            .GetFromJsonAsync<List<CommandResponse>>("/api/status/commands", Ct);

        Assert.NotNull(commands);
        Assert.Equal(["fab", "profile"], commands.Select(c => c.Name));
    }

    [Fact]
    public async Task Commands_WithLocale_ReturnsTranslatedDetails()
    {
        Factory.Discord.GetApplicationCommandsAsync(Arg.Any<CancellationToken>()).Returns(
        [
            new DiscordApplicationCommand(
                "1", "profile", "Shows a profile", Type: 1,
                DescriptionLocalizations: new Dictionary<string, string> { ["pt-BR"] = "Mostra um perfil" },
                Contexts: [0],
                Options: [new DiscordCommandOption(6, "user", "Selected user")]),
        ]);

        var commands = await Factory.CreateClientAs(null)
            .GetFromJsonAsync<List<CommandResponse>>("/api/status/commands?locale=pt-BR", Ct);

        var command = Assert.Single(commands!);
        Assert.Equal("Mostra um perfil", command.Description);
        Assert.True(command.GuildOnly);
        var option = Assert.Single(command.Options);
        Assert.Equal("user", option.Type);
        Assert.False(option.Required);
        Assert.Equal("Selected user", option.Description); // sem tradução: padrão
    }

    private void GivenCommands(params string[] names) =>
        Factory.Discord.GetApplicationCommandsAsync(Arg.Any<CancellationToken>())
            .Returns(names.Select((name, i) => new DiscordApplicationCommand($"{i}", name, name, Type: 1)).ToList());

    private async Task GivenHeartbeatAsync(DateTimeOffset updatedAt, int guildCount)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.BotStatus.Add(new BotStatus
        {
            LatencyMs = 42,
            GuildCount = guildCount,
            StartedAt = updatedAt.AddHours(-1),
            UpdatedAt = updatedAt,
        });
        await db.SaveChangesAsync(Ct);
    }
}
