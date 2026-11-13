using Bangboo.Api.Common.Discord;

namespace Bangboo.Api.Tests.Unit;

public sealed class DiscordPermissionsTests
{
    private static DiscordPartialGuild Guild(bool owner = false, string? permissions = "0") =>
        new("1", "Guild", null, null, owner, permissions, null, null);

    [Fact]
    public void CanManage_Owner_ReturnsTrue() =>
        Assert.True(DiscordPermissions.CanManage(Guild(owner: true)));

    [Theory]
    [InlineData("8")]   // Administrator
    [InlineData("32")]  // Manage Server
    [InlineData("40")]  // ambos
    public void CanManage_AdminOrManageGuild_ReturnsTrue(string permissions) =>
        Assert.True(DiscordPermissions.CanManage(Guild(permissions: permissions)));

    [Theory]
    [InlineData("0")]
    [InlineData("2048")] // Send Messages
    [InlineData(null)]
    [InlineData("not-a-number")]
    public void CanManage_WithoutPermission_ReturnsFalse(string? permissions) =>
        Assert.False(DiscordPermissions.CanManage(Guild(permissions: permissions)));
}
