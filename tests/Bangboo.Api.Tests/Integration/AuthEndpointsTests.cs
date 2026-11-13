using System.Net;
using System.Net.Http.Json;
using Bangboo.Api.Dtos;

namespace Bangboo.Api.Tests.Integration;

public sealed class AuthEndpointsTests(BangbooApiFactory factory) : ApiTestBase(factory)
{
    [Fact]
    public async Task Me_Authenticated_ReturnsUser()
    {
        var user = await Factory.CreateClientAs(UserId).GetFromJsonAsync<UserResponse>("/api/auth/me", Ct);

        Assert.NotNull(user);
        Assert.Equal(UserId, user.Id);
        Assert.Equal("tester", user.Username);
        Assert.Equal("Tester", user.GlobalName);
        Assert.StartsWith("https://cdn.discordapp.com/embed/avatars/", user.AvatarUrl);
    }

    [Fact]
    public async Task Me_Anonymous_Returns401()
    {
        var response = await Factory.CreateClientAs(null).GetAsync("/api/auth/me", Ct);

        await AssertApiErrorAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_RedirectsToDiscord()
    {
        var response = await Factory.CreateClientAs(null).GetAsync("/api/auth/login?returnUrl=/dashboard", Ct);

        Assert.Equal(HttpStatusCode.Found, response.StatusCode);
        Assert.Equal("discord.com", response.Headers.Location?.Host);
    }

    [Fact]
    public async Task Logout_IsIdempotent()
    {
        var response = await Factory.CreateClientAs(null).PostAsync("/api/auth/logout", content: null, Ct);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
