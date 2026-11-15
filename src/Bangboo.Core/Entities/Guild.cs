namespace Bangboo.Core.Entities;

public sealed class Guild
{
    public ulong Id { get; set; }
    public required string Name { get; set; }
    public string? Icon { get; set; }
    public string? Banner { get; set; }
    public uint XpPerMessage { get; set; }
    public ulong? LogsChannelId { get; set; }

    public ulong? OwnerId { get; set; }
    public User? Owner { get; set; }

    public ICollection<Member> Members { get; set; } = [];
    public GuildMemberEvents? MemberEvents { get; set; }
    public Mousetrap? Mousetrap { get; set; }
    public ICollection<ModeratorRole> ModeratorRoles { get; set; } = [];
}
