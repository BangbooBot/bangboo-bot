using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Bangboo.Api.Common.Discord;

public static class DiscordExtensions
{
    public static IServiceCollection AddDiscordApi(this IServiceCollection services)
    {
        var options = services.AddOptions<DiscordOptions>()
            .BindConfiguration(DiscordOptions.SectionName)
            .ValidateDataAnnotations();

        if (!BuildContext.IsGeneratingOpenApi)
        {
            options.ValidateOnStart();
        }

        services.AddMemoryCache();

        services.AddHttpClient<DiscordApiClient>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<DiscordOptions>>().Value;
            client.BaseAddress = new Uri(options.ApiBaseUrl);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("BangbooDashboard (https://github.com/BangbooBot, 1.0)");
        });

        services.AddScoped<IDiscordApiClient>(provider => new CachedDiscordApiClient(
            provider.GetRequiredService<DiscordApiClient>(),
            provider.GetRequiredService<IMemoryCache>()));

        return services;
    }
}
