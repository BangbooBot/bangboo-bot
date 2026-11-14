using System.Net;
using System.Net.Http.Json;
using Bangboo.Api.Common.Discord;
using Bangboo.Api.Common.Errors;
using Bangboo.Api.Dtos;
using Bangboo.Core.Entities;

namespace Bangboo.Api.Tests.Integration;

public sealed class MousetrapEndpointsTests(BangbooApiFactory factory) : ApiTestBase(factory)
{
    private const string GuildId = "10";
    private const string Url = $"/api/guilds/{GuildId}/mousetrap";

    private void GivenManageableGuildWithBot(string permissions = "32")
    {
        Factory.Discord.GetUserGuildsAsync(TestAuthHandler.AccessToken, Arg.Any<CancellationToken>())
            .Returns([new DiscordPartialGuild(GuildId, "Bangboo HQ", null, null, Owner: false, permissions, 3, 1)]);
        Factory.Discord.GetGuildAsync(GuildId, Arg.Any<CancellationToken>())
            .Returns(new DiscordGuild(GuildId, "Bangboo HQ", null, null, null, "999", 2, 7, 120, 40, "pt-BR",
                [new DiscordRole(GuildId, "@everyone"), new DiscordRole("r1", "Mods")]));
        Factory.Discord.GetGuildChannelsAsync(GuildId, Arg.Any<CancellationToken>()).Returns(
        [
            new DiscordChannel("100", 0, "trap", 0, null),
            new DiscordChannel("200", 2, "voice", 1, null),
        ]);
    }

    private static UpdateMousetrapRequest ValidRequest() => new()
    {
        Enabled = true,
        ChannelId = "100",
        Action = MousetrapAction.ByTenure,
        MinTenureDays = 30,
        SendDm = true,
        DmMessage = "Hi {user}, you were removed from {server}.",
    };

    [Fact]
    public async Task Get_NeverConfigured_ReturnsDisabledDefault()
    {
        GivenManageableGuildWithBot();

        var settings = await Factory.CreateClientAs(UserId).GetFromJsonAsync<MousetrapResponse>(Url, Json, Ct);

        Assert.NotNull(settings);
        Assert.False(settings.Enabled);
        Assert.Null(settings.ChannelId);
        Assert.Equal(MousetrapAction.Ban, settings.Action);
    }

    [Fact]
    public async Task Put_Valid_PersistsAndReturnsChannelName()
    {
        GivenManageableGuildWithBot();
        var client = Factory.CreateClientAs(UserId);

        var response = await client.PutAsJsonAsync(Url, ValidRequest(), Json, Ct);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var saved = await client.GetFromJsonAsync<MousetrapResponse>(Url, Json, Ct);
        Assert.NotNull(saved);
        Assert.True(saved.Enabled);
        Assert.Equal("100", saved.ChannelId);
        Assert.Equal("trap", saved.ChannelName);
        Assert.Equal(MousetrapAction.ByTenure, saved.Action);
        Assert.Equal(30, saved.MinTenureDays);
        Assert.True(saved.SendDm);
    }

    [Fact]
    public async Task Put_SerializesActionAsCamelCaseText()
    {
        GivenManageableGuildWithBot();

        var response = await Factory.CreateClientAs(UserId).PutAsJsonAsync(Url, ValidRequest(), Json, Ct);
        var json = await response.Content.ReadAsStringAsync(Ct);

        Assert.Contains("\"action\":\"byTenure\"", json);
    }

    [Fact]
    public async Task Put_NonTenureAction_ClearsMinTenure()
    {
        GivenManageableGuildWithBot();

        var response = await Factory.CreateClientAs(UserId)
            .PutAsJsonAsync(Url, ValidRequest() with { Action = MousetrapAction.Kick }, Json, Ct);
        var saved = await response.Content.ReadFromJsonAsync<MousetrapResponse>(Json, Ct);

        Assert.Equal(MousetrapAction.Kick, saved!.Action);
        Assert.Null(saved.MinTenureDays);
    }

    public static TheoryData<string, UpdateMousetrapRequest> InvalidRequests => new()
    {
        { "enabled without channel", new UpdateMousetrapRequest { Enabled = true } },
        { "voice channel", ValidRequest() with { ChannelId = "200" } },
        { "unknown channel", ValidRequest() with { ChannelId = "404" } },
        { "tenure without days", ValidRequest() with { MinTenureDays = null } },
        { "tenure out of range", ValidRequest() with { MinTenureDays = 400 } },
        { "dm without message", ValidRequest() with { DmMessage = "  " } },
        { "dm too long", ValidRequest() with { DmMessage = new string('a', 1001) } },
    };

    [Theory]
    [MemberData(nameof(InvalidRequests))]
    public async Task Put_Invalid_Returns400ApiError(string scenario, UpdateMousetrapRequest request)
    {
        Assert.NotEmpty(scenario);
        GivenManageableGuildWithBot();

        var response = await Factory.CreateClientAs(UserId).PutAsJsonAsync(Url, request, Json, Ct);

        await AssertApiErrorAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_WithoutManagePermission_Returns403()
    {
        GivenManageableGuildWithBot(permissions: "0");

        var response = await Factory.CreateClientAs(UserId).PutAsJsonAsync(Url, ValidRequest(), Json, Ct);

        await AssertApiErrorAsync(response, HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Get_BotNotInGuild_Returns404()
    {
        GivenManageableGuildWithBot();
        Factory.Discord.GetGuildChannelsAsync(GuildId, Arg.Any<CancellationToken>())
            .Returns<IReadOnlyList<DiscordChannel>>(_ => throw new NotFoundException("Bangboo is not in this server"));

        var response = await Factory.CreateClientAs(UserId).GetAsync(Url, Ct);

        var error = await AssertApiErrorAsync(response, HttpStatusCode.NotFound);
        Assert.Equal("Bangboo is not in this server", error.Message);
    }

    [Fact]
    public async Task Overview_ReturnsCountsAndMousetrap()
    {
        GivenManageableGuildWithBot();
        await Factory.CreateClientAs(UserId).PutAsJsonAsync(Url, ValidRequest(), Json, Ct);

        var overview = await Factory.CreateClientAs(UserId)
            .GetFromJsonAsync<GuildOverviewResponse>($"/api/guilds/{GuildId}/overview", Json, Ct);

        Assert.NotNull(overview);
        Assert.Equal("Bangboo HQ", overview.Name);
        Assert.Equal(120, overview.MemberCount);
        Assert.Equal(1, overview.TextChannelCount);
        Assert.Equal(1, overview.VoiceChannelCount);
        Assert.Equal(1, overview.RoleCount); // sem @everyone
        Assert.True(overview.Mousetrap.Enabled);
        Assert.Equal("trap", overview.Mousetrap.ChannelName);
    }
}
