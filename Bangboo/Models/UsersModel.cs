using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bangboo.Models;

[Table("users")]
public class UsersModel
{
    [Key]
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public ulong Id { get; set; }
    
    [Required]
    [Column("username")]
    public required string Username { get; set; }
    
    [Column("avatar")]
    public string? Avatar { get; set; }

    public AuthsModel? AuthModel { get; set; } = null;
    
    public ICollection<GuildsModel> GuildsModel { get; set; } = new List<GuildsModel>();
    
    public ICollection<MembersModel> MembersModel { get; set; } = new List<MembersModel>();
}