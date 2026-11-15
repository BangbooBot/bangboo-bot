using Bangboo.Api.Common.Discord;
using Bangboo.Api.Services;

namespace Bangboo.Api.Tests.Unit;

public sealed class GuildRoleMapperTests
{
    private static DiscordGuild GuildWith(params DiscordRole[] roles) =>
        new("1", "Guild", null, null, null, "9", 0, null, null, null, null, roles);

    [Fact]
    public void ToAssignableRoles_SkipsEveryoneAndManaged_HighestFirst()
    {
        var guild = GuildWith(
            new DiscordRole("1", "@everyone"),
            new DiscordRole("2", "Low", 0, 1),
            new DiscordRole("3", "High", 0xFF0000, 5),
            new DiscordRole("4", "Bot", 0, 6, Managed: true));

        var result = GuildRoleMapper.ToAssignableRoles(guild);

        Assert.Equal(["3", "2"], result.Select(role => role.Id));
        Assert.Equal(0xFF0000, result[0].Color);
    }

    [Fact]
    public void ToAssignableRoles_NoRoles_ReturnsEmpty()
    {
        Assert.Empty(GuildRoleMapper.ToAssignableRoles(GuildWith()));
    }
}
