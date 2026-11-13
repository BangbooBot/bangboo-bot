using Bangboo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bangboo.Core.Data.Configurations;

public sealed class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("members");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id).HasColumnName("id").UseIdentityByDefaultColumn();
        builder.Property(m => m.Xp).HasColumnName("xp").HasDefaultValue(0u);
        builder.Property(m => m.GuildId).HasColumnName("fk_guild_id");
        builder.Property(m => m.UserId).HasColumnName("fk_user_id");

        builder.HasIndex(m => new { m.GuildId, m.UserId }).IsUnique();

        builder.HasOne(m => m.Guild)
            .WithMany(g => g.Members)
            .HasForeignKey(m => m.GuildId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.User)
            .WithMany(u => u.Memberships)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
