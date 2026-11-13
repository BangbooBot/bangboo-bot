using Bangboo.Core.Data;
using Bangboo.Core.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bangboo.Core.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra o <see cref="IDbContextFactory{TContext}"/> (singleton, para serviços singleton/background
    /// como o heartbeat do bot) e o <see cref="AppDbContext"/> (scoped, para requests da API).
    /// </summary>
    public static IServiceCollection AddBangbooDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // A connection string é lida só quando o contexto é criado: assim a geração do
        // OpenAPI no build (que monta o host sem configuração) não falha.
        services.AddDbContextFactory<AppDbContext>(
            options => options.UseNpgsql(
                configuration.GetConnectionString(DatabaseOptions.ConnectionStringName)
                ?? throw new InvalidOperationException(
                    $"Connection string '{DatabaseOptions.ConnectionStringName}' is not configured.")));

        return services;
    }
}
