namespace Bangboo.Core.Entities;

public sealed class Member
{
    public long Id { get; set; }
    public uint Xp { get; set; }

    public ulong GuildId { get; set; }
    public Guild Guild { get; set; } = null!;

    public ulong UserId { get; set; }
    public User User { get; set; } = null!;
}
