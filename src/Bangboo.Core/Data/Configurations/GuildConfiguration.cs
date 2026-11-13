using Bangboo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bangboo.Core.Data.Configurations;

public sealed class GuildConfiguration : IEntityTypeConfiguration<Guild>
{
    public void Configure(EntityTypeBuilder<Guild> builder)
    {
        builder.ToTable("guilds");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(g => g.Name).HasColumnName("name").HasMaxLength(100);
        builder.Property(g => g.Icon).HasColumnName("icon");
        builder.Property(g => g.Banner).HasColumnName("banner");
        builder.Property(g => g.XpPerMessage).HasColumnName("xp_per_message").HasDefaultValue(0u);
        builder.Property(g => g.LogsChannelId).HasColumnName("logs_channel_id");
        builder.Property(g => g.OwnerId).HasColumnName("fk_owner_id");

        // O dono pode nunca ter logado no dashboard, por isso a FK é opcional.
        builder.HasOne(g => g.Owner)
            .WithMany(u => u.OwnedGuilds)
            .HasForeignKey(g => g.OwnerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
