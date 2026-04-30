using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("sessions")]
public class SessionsModel
{
    [Key]
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public ulong Id { get; set; }
    
    [Required]
    [Column("expire_in")]
    public DateOnly ExpiresIn { get; set; }

    [Required]
    [Column("user_agent")]
    public required string UserAgent { get; set; }

    [Required]
    [Column("platform")]
    public required string Platform { get; set; }

    [Required]
    [Column("language")]
    public required string Language { get; set; }

    [Column("avatar")]
    public string? Avatar { get; set; }

    [Required]
    [Column("fk_user_id")]
    public ulong FkUserId;

    public UsersModel UserModel { get; set; } = null!;
}