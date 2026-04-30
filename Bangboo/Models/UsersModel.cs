using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("users")]
public class UsersModel
{
    [Key]
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

    public ICollection<SessionsModel> Sessions { get; set; } = new List<SessionsModel>();    
}