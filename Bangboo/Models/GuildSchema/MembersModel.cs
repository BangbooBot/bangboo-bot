using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bangboo.Models.GuildSchema;

[Table("members")]
public class MembersModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public ulong Id { get; set; }

    [Column("xp")]
    [DefaultValue(0)]
    public uint Xp { get; set; } = 0;
    
    [Column("fk_guild_id")]
    public ulong FkGuildId { get; set; }
    
    public GuildsModel GuildModel { get; set; } = null!;
    
    [Column("fk_user_id")]
    public ulong FkUserId { get; set; }
    
    public UsersModel UserModel { get; set; } = null!;
}