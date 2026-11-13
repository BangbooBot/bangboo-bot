namespace Bangboo.Core.Entities;

/// <summary>Quais eventos de membro a guild anuncia e em quais canais.</summary>
public sealed class GuildMemberEvents
{
    public long Id { get; set; }
    public bool OnJoin { get; set; }
    public bool OnLeave { get; set; }
    public bool OnBan { get; set; }
    public bool OnBoost { get; set; }
    public bool OnMute { get; set; }
    public ulong? SystemChannelId { get; set; }
    public ulong? BoostChannelId { get; set; }
    public ulong? ModChannelId { get; set; }

    public ulong GuildId { get; set; }
    public Guild Guild { get; set; } = null!;
}
