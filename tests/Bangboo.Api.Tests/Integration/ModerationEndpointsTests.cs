using System.Net;
using System.Net.Http.Json;
using Bangboo.Api.Common.Discord;
using Bangboo.Api.Common.Errors;
using Bangboo.Api.Dtos;

namespace Bangboo.Api.Tests.Integration;

public sealed class ModerationEndpointsTests(BangbooApiFactory factory) : ApiTestBase(factory)
{
    private const string GuildId = "10";
    private const string Url = $"/api/guilds/{GuildId}/moderation";

    private void GivenManageableGuildWithBot(string permissions = "32")
    {
        Factory.Discord.GetUserGuildsAsync(TestAuthHandler.AccessToken, Arg.Any<CancellationToken>())
            .Returns([new DiscordPartialGuild(GuildId, "Bangboo HQ", null, null, Owner: false, permissions, 3, 1)]);
        Factory.Discord.GetGuildAsync(GuildId, Arg.Any<CancellationToken>())
            .Returns(new DiscordGuild(GuildId, "Bangboo HQ", null, null, null, "999", 2, 7, 120, 40, "pt-BR",
            [
                new DiscordRole(GuildId, "@everyone"),
                new DiscordRole("20", "Members", 0, 1),
                new DiscordRole("30", "Mods", 0xE91E63, 3),
                new DiscordRole("40", "Helpers", 0x3498DB, 2),
                new DiscordRole("50", "Bangboo", 0, 4, Managed: true),
            ]));
    }

    private static UpdateModerationRequest Request(params string[] roleIds) => new(roleIds);

    [Fact]
    public async Task Get_NeverConfigured_ReturnsEmpty()
    {
        GivenManageableGuildWithBot();

        var settings = await Factory.CreateClientAs(UserId).GetFromJsonAsync<ModerationResponse>(Url, Json, Ct);

        Assert.NotNull(settings);
        Assert.Empty(settings.RoleIds);
    }

    [Fact]
    public async Task Put_Valid_PersistsWithoutDuplicates()
    {
        GivenManageableGuildWithBot();
        var client = Factory.CreateClientAs(UserId);

        var response = await client.PutAsJsonAsync(Url, Request("40", "30", "30"), Json, Ct);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var saved = await client.GetFromJsonAsync<ModerationResponse>(Url, Json, Ct);
        Assert.NotNull(saved);
        Assert.Equal(["30", "40"], saved.RoleIds);
    }

    [Fact]
    public async Task Put_ReplacesPreviousRoles()
    {
        GivenManageableGuildWithBot();
        var client = Factory.CreateClientAs(UserId);
        await client.PutAsJsonAsync(Url, Request("30", "40"), Json, Ct);

        await client.PutAsJsonAsync(Url, Request("20", "30"), Json, Ct);
        var saved = await client.GetFromJsonAsync<ModerationResponse>(Url, Json, Ct);

        Assert.NotNull(saved);
        Assert.Equal(["20", "30"], saved.RoleIds);
    }

    [Fact]
    public async Task Put_EmptyList_ClearsRoles()
    {
        GivenManageableGuildWithBot();
        var client = Factory.CreateClientAs(UserId);
        await client.PutAsJsonAsync(Url, Request("30"), Json, Ct);

        await client.PutAsJsonAsync(Url, Request(), Json, Ct);
        var saved = await client.GetFromJsonAsync<ModerationResponse>(Url, Json, Ct);

        Assert.NotNull(saved);
        Assert.Empty(saved.RoleIds);
    }

    [Theory]
    [InlineData("999")] // não existe
    [InlineData(GuildId)] // @everyone
    [InlineData("50")] // cargo de bot
    public async Task Put_NonAssignableRole_Returns400(string roleId)
    {
        GivenManageableGuildWithBot();

        var response = await Factory.CreateClientAs(UserId).PutAsJsonAsync(Url, Request("30", roleId), Json, Ct);

        await AssertApiErrorAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_TooManyRoles_Returns400()
    {
        GivenManageableGuildWithBot();
        var roleIds = Enumerable.Range(0, 26).Select(i => "30").ToArray();

        var response = await Factory.CreateClientAs(UserId).PutAsJsonAsync(Url, Request(roleIds), Json, Ct);

        await AssertApiErrorAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_WithoutManagePermission_Returns403()
    {
        GivenManageableGuildWithBot(permissions: "0");

        var response = await Factory.CreateClientAs(UserId).PutAsJsonAsync(Url, Request("30"), Json, Ct);

        await AssertApiErrorAsync(response, HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Get_BotNotInGuild_Returns404()
    {
        GivenManageableGuildWithBot();
        Factory.Discord.GetGuildAsync(GuildId, Arg.Any<CancellationToken>())
            .Returns<DiscordGuild>(_ => throw new NotFoundException("Bangboo is not in this server"));

        var response = await Factory.CreateClientAs(UserId).GetAsync(Url, Ct);

        await AssertApiErrorAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Roles_ReturnsAssignableRolesFromHighestToLowest()
    {
        GivenManageableGuildWithBot();

        var roles = await Factory.CreateClientAs(UserId)
            .GetFromJsonAsync<List<GuildRoleResponse>>($"/api/guilds/{GuildId}/roles", Json, Ct);

        Assert.NotNull(roles);
        Assert.Equal(["30", "40", "20"], roles.Select(role => role.Id));
        Assert.Equal(0xE91E63, roles[0].Color);
    }
}
