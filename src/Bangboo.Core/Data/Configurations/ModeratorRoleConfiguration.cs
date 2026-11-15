using Bangboo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bangboo.Core.Data.Configurations;

public sealed class ModeratorRoleConfiguration : IEntityTypeConfiguration<ModeratorRole>
{
    public void Configure(EntityTypeBuilder<ModeratorRole> builder)
    {
        builder.ToTable("moderator_roles");
        builder.HasKey(r => new { r.GuildId, r.RoleId });

        builder.Property(r => r.GuildId).HasColumnName("fk_guild_id");
        builder.Property(r => r.RoleId).HasColumnName("role_id");

        builder.HasOne(r => r.Guild)
            .WithMany(g => g.ModeratorRoles)
            .HasForeignKey(r => r.GuildId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
