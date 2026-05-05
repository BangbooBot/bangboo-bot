using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bangboo.Models;

[Table("mousetraps")]
public class MousetrapsModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }
    
    [Column("channel_id")]
    [Required]
    public ulong ChannelId { get; set; }

    [Column("should_ban")] 
    [DefaultValue(false)] 
    public bool ShouldBan { get; set; } = false;
    
    [Column("message_dm")] 
    public string? MessageDm { get; set; }
    
    [Column("fk_guild_id")]
    public ulong FkGuildId { get; set; }

    public GuildsModel GuildModel = null!;
}