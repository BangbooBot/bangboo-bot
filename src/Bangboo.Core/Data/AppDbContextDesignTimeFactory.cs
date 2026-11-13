using Bangboo.Core.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Bangboo.Core.Data;

/// <summary>
/// Usada só pelo <c>dotnet ef</c>. Permite rodar migrations sem projeto de startup.
/// Sobrescreva a conexão com a variável de ambiente <c>ConnectionStrings__DbConnection</c>.
/// </summary>
public sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string LocalConnectionString =
        "Host=localhost;Port=5432;Database=bangboo;Username=bangboo;Password=bangboo";

    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable($"ConnectionStrings__{DatabaseOptions.ConnectionStringName}")
            ?? LocalConnectionString;

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
