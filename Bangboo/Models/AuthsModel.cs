using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bangboo.Models;

[Table("auths")]
public class AuthsModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public ulong Id { get; set; }
    
    [Required]
    [Column("access_token")]
    public required string AccessToken { get; set; }

    [Required]
    [Column("refresh_token")]
    public required string RefreshToken { get; set; }

    [Required]
    [Column("token_type")]
    public required string TokenType { get; set; }

    [Required]
    [Column("expires_in")]
    public DateOnly ExpiresIn { get; set; }

    [Required]
    [Column("scope")]
    public required string Scope { get; set; }
    
    [Required]
    [Column("fk_user")]
    public required ulong FkUserId { get; set; }

    public UsersModel UserModel { get; set; } = null!; 
    
    public ICollection<SessionsModel> SessionsModel { get; set; } = new List<SessionsModel>();  
}