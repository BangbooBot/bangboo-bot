using Bangboo.Api.Common.Discord;
using Bangboo.Api.Services;

namespace Bangboo.Api.Tests.Unit;

public sealed class GuildChannelMapperTests
{
    [Fact]
    public void ToTextChannels_KeepsTextAndAnnouncements_InDiscordOrder()
    {
        DiscordChannel[] channels =
        [
            new("c2", 4, "Second category", 2, null),
            new("c1", 4, "First category", 1, null),
            new("t3", 0, "chat", 0, "c2"),
            new("t2", 5, "news", 1, "c1"),
            new("t1", 0, "rules", 0, "c1"),
            new("v1", 2, "voice", 0, "c1"),
            new("t0", 0, "lobby", 5, null),
        ];

        var result = GuildChannelMapper.ToTextChannels(channels);

        Assert.Equal(["t0", "t1", "t2", "t3"], result.Select(channel => channel.Id));
        Assert.Null(result[0].Category);
        Assert.Equal("First category", result[1].Category);
        Assert.Equal("Second category", result[3].Category);
    }
}
