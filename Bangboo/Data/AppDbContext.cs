using Bangboo.Models;
using Bangboo.Models;
using Microsoft.EntityFrameworkCore;

namespace Bangboo.Data;

public class AppDbContext : DbContext
{
    public DbSet<UsersModel> Users { get; set; }
    
    public DbSet<AuthsModel> Auths { get; set; }
    
    public DbSet<SessionsModel> Sessions { get; set; }
    
    public DbSet<GuildsModel> Guilds { get; set; }
    
    public DbSet<MembersModel> Members { get; set; }
    
    public DbSet<MemberEventsModel> MemberEvents { get; set; }
    
    public DbSet<MousetrapsModel> MousetrapsModel { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { 
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        /*
        modelBuilder.HasPostgresEnum<ERecursoType>("e_recurso_type");
        modelBuilder.HasPostgresEnum<EHorarioType>("e_horario_type");

        modelBuilder.Entity<Espacos>()
            .Property(p => p.Recursos)
            .HasColumnType("e_recurso_type[]");

        modelBuilder.Entity<Reservas>()
            .Property(p => p.Horarios)
            .HasColumnType("e_horario_type[]");
        */

        // Auths
        modelBuilder.Entity<AuthsModel>()
            .Property(e => e.Id)
            .UseIdentityByDefaultColumn();
            
        modelBuilder.Entity<AuthsModel>()
            .HasOne(r => r.UserModel)
            .WithOne(e => e.AuthModel)
            .HasForeignKey<AuthsModel>(r => r.FkUserId);
        
        // Sessions
        modelBuilder.Entity<SessionsModel>()
            .HasOne(r => r.AuthModel)
            .WithMany(e => e.SessionsModel)
            .HasForeignKey(r => r.FkAuthId);
        
        // Guilds
        modelBuilder.Entity<GuildsModel>()
            .HasOne(r => r.UsersModel)
            .WithMany(e => e.GuildsModel)
            .HasForeignKey(r => r.FkOwnerId);
        
        // Members
        modelBuilder.Entity<MembersModel>()
            .Property(e => e.Id)
            .UseIdentityByDefaultColumn();
        
        modelBuilder.Entity<MembersModel>()
            .HasOne(r => r.UserModel)
            .WithMany(e => e.MembersModel)
            .HasForeignKey(r => r.FkUserId);
        
        // Members Events
        modelBuilder.Entity<MemberEventsModel>()
            .Property(e => e.Id)
            .UseIdentityByDefaultColumn();
        
        modelBuilder.Entity<MemberEventsModel>()
            .HasOne(r => r.GuildModel)
            .WithOne(e => e.MemberEventModel)
            .HasForeignKey<MemberEventsModel>(r => r.FkGuildId);
        
        // Mousetraps
        modelBuilder.Entity<MousetrapsModel>()
            .Property(e => e.Id)
            .UseIdentityByDefaultColumn();
        
        modelBuilder.Entity<MousetrapsModel>()
            .HasOne(r => r.GuildModel)
            .WithOne(e => e.MousetrapsModel)
            .HasForeignKey<MousetrapsModel>(r => r.FkGuildId);
    }
}