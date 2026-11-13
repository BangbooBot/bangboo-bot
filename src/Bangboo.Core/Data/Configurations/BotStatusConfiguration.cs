using Bangboo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bangboo.Core.Data.Configurations;

public sealed class BotStatusConfiguration : IEntityTypeConfiguration<BotStatus>
{
    public void Configure(EntityTypeBuilder<BotStatus> builder)
    {
        builder.ToTable("bot_status");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(s => s.LatencyMs).HasColumnName("latency_ms");
        builder.Property(s => s.GuildCount).HasColumnName("guild_count");
        builder.Property(s => s.StartedAt).HasColumnName("started_at");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");
    }
}
