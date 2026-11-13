namespace Bangboo.Core.Entities;

/// <summary>Canal-armadilha: quem envia mensagem nele é banido (automod).</summary>
public sealed class Mousetrap
{
    public long Id { get; set; }
    public ulong ChannelId { get; set; }
    public bool ShouldBan { get; set; }
    public string? MessageDm { get; set; }

    public ulong GuildId { get; set; }
    public Guild Guild { get; set; } = null!;
}
