using Bangboo.Api.Services;
using Bangboo.Api.Services.Interfaces;

namespace Bangboo.Api.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra os services da aplicação (<c>Services/</c>) contra suas interfaces
    /// (<c>Services/Interfaces/</c>). Service novo = uma linha aqui.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IGuildService, GuildService>();
        services.AddScoped<IStatusService, StatusService>();

        return services;
    }
}
