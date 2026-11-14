using Bangboo.Api.Common.Discord;
using Bangboo.Core.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute.ClearExtensions;
using Testcontainers.PostgreSql;

namespace Bangboo.Api.Tests.Integration;

/// <summary>
/// API real + PostgreSQL em container. O Discord é um substitute (<see cref="Discord"/>) e a
/// autenticação usa <see cref="TestAuthHandler"/>.
/// </summary>
public sealed class BangbooApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:18-alpine")
        .Build();

    public IDiscordApiClient Discord { get; } = Substitute.For<IDiscordApiClient>();

    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync();

        await using var scope = Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
    }

    public new async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    /// <summary>Client autenticado como <paramref name="userId"/>, ou anônimo se null.</summary>
    public HttpClient CreateClientAs(string? userId)
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        if (userId is not null)
        {
            client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, userId);
        }

        return client;
    }

    public async Task ResetAsync()
    {
        Discord.ClearSubstitute(ClearOptions.All);

        await using var scope = Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.BotStatus.ExecuteDeleteAsync();
        await db.Mousetraps.ExecuteDeleteAsync();
        await db.Guilds.ExecuteDeleteAsync();
        await db.Users.ExecuteDeleteAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:DbConnection", _postgres.GetConnectionString());
        builder.UseSetting("Discord:ClientId", "123");
        builder.UseSetting("Discord:ClientSecret", "secret");
        builder.UseSetting("Discord:BotToken", "bot-token");

        builder.ConfigureTestServices(services =>
        {
            services.AddScoped(_ => Discord);
            services.AddAuthentication(options => options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }
}
