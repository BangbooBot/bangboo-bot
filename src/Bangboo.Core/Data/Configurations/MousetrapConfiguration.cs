using Bangboo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bangboo.Core.Data.Configurations;

public sealed class MousetrapConfiguration : IEntityTypeConfiguration<Mousetrap>
{
    public void Configure(EntityTypeBuilder<Mousetrap> builder)
    {
        builder.ToTable("mousetraps");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id).HasColumnName("id").UseIdentityByDefaultColumn();
        builder.Property(m => m.Enabled).HasColumnName("enabled").HasDefaultValue(false);
        builder.Property(m => m.ChannelId).HasColumnName("channel_id");
        builder.Property(m => m.Action)
            .HasColumnName("action")
            .HasConversion<string>()
            .HasMaxLength(16)
            .HasDefaultValue(MousetrapAction.Ban);
        builder.Property(m => m.MinTenureDays).HasColumnName("min_tenure_days");
        builder.Property(m => m.SendDm).HasColumnName("send_dm").HasDefaultValue(false);
        builder.Property(m => m.DmMessage).HasColumnName("message_dm").HasMaxLength(Mousetrap.MaxDmMessageLength);
        builder.Property(m => m.GuildId).HasColumnName("fk_guild_id");

        builder.HasOne(m => m.Guild)
            .WithOne(g => g.Mousetrap)
            .HasForeignKey<Mousetrap>(m => m.GuildId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
