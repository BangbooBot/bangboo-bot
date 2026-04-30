using Microsoft.EntityFrameworkCore;

namespace bangboo_backend.Data;

public class AppDbContext : DbContext
{
    public DbSet<UsersModel> Users { get; set; }
    
    public DbSet<SessionsModel> Sessions { get; set; }
    
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

        modelBuilder.Entity<SessionsModel>()
            .HasOne(r => r.UserModel)
            .WithMany(e => e.Sessions)
            .HasForeignKey(r => r.FkUserId);
    }
}