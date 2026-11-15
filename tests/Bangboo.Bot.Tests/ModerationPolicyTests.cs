using Bot.Services.Moderation;

namespace Bangboo.Bot.Tests;

public sealed class ModerationPolicyTests
{
    private static readonly IReadOnlySet<ulong> Allowed = new HashSet<ulong> { 30, 40 };

    [Fact]
    public void CanModerate_Owner_WithoutRoles_IsAllowed() =>
        Assert.True(ModerationPolicy.CanModerate(isOwner: true, isAdministrator: false, [], new HashSet<ulong>()));

    [Fact]
    public void CanModerate_Administrator_WithoutConfiguredRoles_IsAllowed() =>
        Assert.True(ModerationPolicy.CanModerate(isOwner: false, isAdministrator: true, [20], new HashSet<ulong>()));

    [Fact]
    public void CanModerate_MemberWithAllowedRole_IsAllowed() =>
        Assert.True(ModerationPolicy.CanModerate(isOwner: false, isAdministrator: false, [20, 40], Allowed));

    [Fact]
    public void CanModerate_MemberWithoutAllowedRole_IsDenied() =>
        Assert.False(ModerationPolicy.CanModerate(isOwner: false, isAdministrator: false, [20], Allowed));

    [Fact]
    public void CanModerate_NoRolesConfigured_OnlyOwnerAndAdmins() =>
        Assert.False(ModerationPolicy.CanModerate(isOwner: false, isAdministrator: false, [20, 30], new HashSet<ulong>()));
}
