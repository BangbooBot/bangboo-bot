using Bangboo.Core.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bangboo.Core.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IDataProtectionKeyContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Guild> Guilds => Set<Guild>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<GuildMemberEvents> GuildMemberEvents => Set<GuildMemberEvents>();
    public DbSet<Mousetrap> Mousetraps => Set<Mousetrap>();
    public DbSet<BotStatus> BotStatus => Set<BotStatus>();

    /// <summary>Chaves do ASP.NET Data Protection (cookies de auth sobrevivem a restart).</summary>
    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
