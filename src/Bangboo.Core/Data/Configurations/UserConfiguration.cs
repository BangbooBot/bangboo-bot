using Bangboo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bangboo.Core.Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(u => u.Username).HasColumnName("username").HasMaxLength(32);
        builder.Property(u => u.GlobalName).HasColumnName("global_name").HasMaxLength(32);
        builder.Property(u => u.Avatar).HasColumnName("avatar");
        builder.Property(u => u.LastLoginAt).HasColumnName("last_login_at");
    }
}
