using Bangboo.Core.Entities;
using Bot.Services.Mousetrap;

namespace Bangboo.Bot.Tests;

public sealed class MousetrapPolicyTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 14, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Decide_Ban_AlwaysBans() =>
        Assert.Equal(MousetrapPunishment.Ban,
            MousetrapPolicy.Decide(MousetrapAction.Ban, null, Now.AddYears(-3), Now));

    [Fact]
    public void Decide_Kick_AlwaysKicks() =>
        Assert.Equal(MousetrapPunishment.Kick,
            MousetrapPolicy.Decide(MousetrapAction.Kick, null, Now.AddMinutes(-1), Now));

    [Theory]
    [InlineData(30, MousetrapPunishment.Kick)] // exatamente no limite: expulsa
    [InlineData(31, MousetrapPunishment.Kick)]
    [InlineData(29, MousetrapPunishment.Ban)]
    [InlineData(0, MousetrapPunishment.Ban)]
    public void Decide_ByTenure_KicksVeteransAndBansNewcomers(int daysInServer, MousetrapPunishment expected) =>
        Assert.Equal(expected,
            MousetrapPolicy.Decide(MousetrapAction.ByTenure, 30, Now.AddDays(-daysInServer), Now));

    [Fact]
    public void Decide_ByTenure_UnknownJoinDate_Bans() =>
        Assert.Equal(MousetrapPunishment.Ban,
            MousetrapPolicy.Decide(MousetrapAction.ByTenure, 30, joinedAt: null, Now));

    [Fact]
    public void FormatMessage_ReplacesPlaceholders() =>
        Assert.Equal(
            "Hi nathan, you were removed from Bangboo HQ.",
            MousetrapPolicy.FormatMessage("Hi {user}, you were removed from {SERVER}.", "nathan", "Bangboo HQ"));
}
