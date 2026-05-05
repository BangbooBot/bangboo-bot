using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bangboo.Models;

[Table("sessions")]
public class SessionsModel
{
    [Key]
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public ulong Id { get; set; }

    [Required]
    [Column("user_agent")]
    public required string UserAgent { get; set; }

    [Required]
    [Column("language")]
    public required string Language { get; set; }

    [Required]
    [Column("fk_auth_id")]
    public long FkAuthId;

    public AuthsModel AuthModel { get; set; } = null!;
}