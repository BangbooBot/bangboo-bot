using Bangboo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bangboo.Core.Data.Configurations;

public sealed class GuildMemberEventsConfiguration : IEntityTypeConfiguration<GuildMemberEvents>
{
    public void Configure(EntityTypeBuilder<GuildMemberEvents> builder)
    {
        builder.ToTable("member_events");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id").UseIdentityByDefaultColumn();
        builder.Property(e => e.OnJoin).HasColumnName("on_join").HasDefaultValue(false);
        builder.Property(e => e.OnLeave).HasColumnName("on_leave").HasDefaultValue(false);
        builder.Property(e => e.OnBan).HasColumnName("on_ban").HasDefaultValue(false);
        builder.Property(e => e.OnBoost).HasColumnName("on_boost").HasDefaultValue(false);
        builder.Property(e => e.OnMute).HasColumnName("on_mute").HasDefaultValue(false);
        builder.Property(e => e.SystemChannelId).HasColumnName("system_channel_id");
        builder.Property(e => e.BoostChannelId).HasColumnName("boost_channel_id");
        builder.Property(e => e.ModChannelId).HasColumnName("mod_channel_id");
        builder.Property(e => e.GuildId).HasColumnName("fk_guild_id");

        builder.HasOne(e => e.Guild)
            .WithOne(g => g.MemberEvents)
            .HasForeignKey<GuildMemberEvents>(e => e.GuildId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
