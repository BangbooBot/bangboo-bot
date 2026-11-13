using System.Net;
using System.Net.Http.Json;
using Bangboo.Api.Common.Discord;
using Bangboo.Api.Dtos;

namespace Bangboo.Api.Tests.Integration;

public sealed class GuildEndpointsTests(BangbooApiFactory factory) : ApiTestBase(factory)
{
    private void GivenGuilds()
    {
        Factory.Discord.GetUserGuildsAsync(TestAuthHandler.AccessToken, Arg.Any<CancellationToken>()).Returns(
        [
            new DiscordPartialGuild("10", "Mine", null, null, Owner: true, "0", 3, 1),
            new DiscordPartialGuild("20", "Other", null, null, Owner: false, "0", 3, 1),
        ]);
        Factory.Discord.GetBotGuildIdsAsync(Arg.Any<CancellationToken>()).Returns(new HashSet<string> { "10" });
    }

    [Fact]
    public async Task List_ReturnsOnlyManageableGuilds()
    {
        GivenGuilds();

        var guilds = await Factory.CreateClientAs(UserId).GetFromJsonAsync<List<GuildResponse>>("/api/guilds", Ct);

        var guild = Assert.Single(guilds!);
        Assert.Equal("10", guild.Id);
        Assert.True(guild.IsBotMember);
    }

    [Fact]
    public async Task Get_ManageableGuild_Returns200()
    {
        GivenGuilds();

        var response = await Factory.CreateClientAs(UserId).GetAsync("/api/guilds/10", Ct);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var guild = await response.Content.ReadFromJsonAsync<GuildResponse>(Ct);
        Assert.Equal("Mine", guild!.Name);
    }
}
