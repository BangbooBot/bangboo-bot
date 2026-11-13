using System.Net;
using Bangboo.Api.Common.Discord;
using Bangboo.Api.Common.Errors;

namespace Bangboo.Api.Tests.Integration;

/// <summary>Toda resposta de erro segue <c>{ status, message }</c>.</summary>
public sealed class ErrorContractTests(BangbooApiFactory factory) : ApiTestBase(factory)
{
    [Fact]
    public async Task PrivateRoute_WithoutSession_Returns401()
    {
        var response = await Factory.CreateClientAs(null).GetAsync("/api/guilds", Ct);

        await AssertApiErrorAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UnknownRoute_Returns404()
    {
        var response = await Factory.CreateClientAs(UserId).GetAsync("/api/does-not-exist", Ct);

        await AssertApiErrorAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ManageGuildPolicy_WithoutPermission_Returns403()
    {
        Factory.Discord.GetUserGuildsAsync(TestAuthHandler.AccessToken, Arg.Any<CancellationToken>())
            .Returns([new DiscordPartialGuild("42", "Guild", null, null, Owner: false, "0", null, null)]);

        var response = await Factory.CreateClientAs(UserId).GetAsync("/api/guilds/42", Ct);

        await AssertApiErrorAsync(response, HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AppException_FromService_KeepsStatusAndMessage()
    {
        Factory.Discord.GetUserGuildsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns<IReadOnlyList<DiscordPartialGuild>>(_ => throw new ExternalServiceException("Discord is down"));

        var response = await Factory.CreateClientAs(UserId).GetAsync("/api/guilds", Ct);

        var error = await AssertApiErrorAsync(response, HttpStatusCode.BadGateway);
        Assert.Equal("Discord is down", error.Message);
    }

    [Fact]
    public async Task UnexpectedException_Returns500WithoutDetails()
    {
        Factory.Discord.GetApplicationCommandsAsync(Arg.Any<CancellationToken>())
            .Returns<IReadOnlyList<DiscordApplicationCommand>>(_ => throw new InvalidOperationException("connection string leaked"));

        var response = await Factory.CreateClientAs(null).GetAsync("/api/status/commands", Ct);

        var error = await AssertApiErrorAsync(response, HttpStatusCode.InternalServerError);
        Assert.DoesNotContain("leaked", error.Message);
    }
}
