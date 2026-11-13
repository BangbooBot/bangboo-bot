using Bangboo.Api.Common.Discord;
using Microsoft.Extensions.Caching.Memory;

namespace Bangboo.Api.Tests.Unit;

public sealed class CachedDiscordApiClientTests
{
    private readonly IDiscordApiClient _inner = Substitute.For<IDiscordApiClient>();
    private readonly CachedDiscordApiClient _client;

    public CachedDiscordApiClientTests()
    {
        _client = new CachedDiscordApiClient(_inner, new MemoryCache(new MemoryCacheOptions()));
    }

    [Fact]
    public async Task GetBotGuildIdsAsync_SecondCall_UsesCache()
    {
        var ct = TestContext.Current.CancellationToken;
        _inner.GetBotGuildIdsAsync(Arg.Any<CancellationToken>()).Returns(new HashSet<string> { "1" });

        await _client.GetBotGuildIdsAsync(ct);
        await _client.GetBotGuildIdsAsync(ct);

        await _inner.Received(1).GetBotGuildIdsAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetUserGuildsAsync_CachesPerToken()
    {
        var ct = TestContext.Current.CancellationToken;
        _inner.GetUserGuildsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns([]);

        await _client.GetUserGuildsAsync("token-a", ct);
        await _client.GetUserGuildsAsync("token-a", ct);
        await _client.GetUserGuildsAsync("token-b", ct);

        await _inner.Received(1).GetUserGuildsAsync("token-a", Arg.Any<CancellationToken>());
        await _inner.Received(1).GetUserGuildsAsync("token-b", Arg.Any<CancellationToken>());
    }
}
