using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bangboo.Models.GuildSchema.EventSchema;
using Bangboo.Models.GuildSchema.ModerationSchema;

namespace Bangboo.Models.GuildSchema;

[Table("guilds")]
public class GuildsModel
{
    [Key]
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public ulong Id { get; set; }
    
    [Required]
    [Column("name")]
    public required string Name { get; set; }
    
    [Column("icon")]
    public string? Icon { get; set; }
    
    [Column("banner")]
    public string? Banner { get; set; }

    [Column("xp_per_message")]
    [DefaultValue(0)]
    public uint XpPerMessage { get; set; } = 0;
    
    [Column("logs_channel_id")]
    public ulong? LogsChannelId { get; set; }
    
    [Column("fk_owner")]
    public ulong FkOwnerId { get; set; }

    public UsersModel UsersModel { get; set; } = null;
    
    public ICollection<MembersModel> MembersModel { get; set; } = new List<MembersModel>();
    
    public MemberEventsModel MemberEventModel { get; set; } = null;
    
    public MousetrapsModel MousetrapsModel { get; set; } = null;
}