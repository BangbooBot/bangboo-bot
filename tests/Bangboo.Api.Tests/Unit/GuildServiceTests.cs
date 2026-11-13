using Bangboo.Api.Common.Auth;
using Bangboo.Api.Common.Discord;
using Bangboo.Api.Common.Errors;
using Bangboo.Api.Services.Interfaces;

using Bangboo.Api.Services;

namespace Bangboo.Api.Tests.Unit;

public sealed class GuildServiceTests
{
    private const string AccessToken = "user-token";

    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IDiscordApiClient _discord = Substitute.For<IDiscordApiClient>();
    private readonly GuildService _service;

    public GuildServiceTests()
    {
        _currentUser.GetDiscordAccessTokenAsync().Returns(AccessToken);
        _discord.GetBotGuildIdsAsync(Arg.Any<CancellationToken>())
            .Returns(new HashSet<string> { "2" });
        _discord.GetUserGuildsAsync(AccessToken, Arg.Any<CancellationToken>()).Returns(
        [
            new DiscordPartialGuild("1", "Bravo", "icon", null, Owner: true, "0", 10, 5),
            new DiscordPartialGuild("2", "Alpha", null, "banner", Owner: false, "32", null, null),
            new DiscordPartialGuild("3", "NoPermission", null, null, Owner: false, "0", 1, 1),
        ]);

        _service = new GuildService(_currentUser, _discord);
    }

    [Fact]
    public async Task GetManageableGuildsAsync_FiltersOutGuildsWithoutPermission()
    {
        var guilds = await _service.GetManageableGuildsAsync(TestContext.Current.CancellationToken);

        Assert.DoesNotContain(guilds, guild => guild.Id == "3");
        Assert.Equal(2, guilds.Count);
    }

    [Fact]
    public async Task GetManageableGuildsAsync_BotGuildsFirstAndMapsFields()
    {
        var guilds = await _service.GetManageableGuildsAsync(TestContext.Current.CancellationToken);

        var first = guilds[0];
        Assert.Equal("2", first.Id);
        Assert.True(first.IsBotMember);
        Assert.Equal("https://cdn.discordapp.com/banners/2/banner.png", first.BannerUrl);
        Assert.Equal(0, first.ApproximateMemberCount);

        var second = guilds[1];
        Assert.Equal("1", second.Id);
        Assert.False(second.IsBotMember);
        Assert.True(second.IsOwner);
        Assert.Equal("https://cdn.discordapp.com/icons/1/icon.png", second.IconUrl);
    }

    [Fact]
    public async Task GetGuildAsync_UnknownGuild_ThrowsNotFound()
    {
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetGuildAsync("999", TestContext.Current.CancellationToken));

        Assert.Equal(404, exception.Status);
    }
}
