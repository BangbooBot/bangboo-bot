namespace Bangboo.Core.Entities;

/// <summary>Usuário do Discord que já fez login no dashboard.</summary>
public sealed class User
{
    public ulong Id { get; set; }
    public required string Username { get; set; }
    public string? GlobalName { get; set; }
    public string? Avatar { get; set; }
    public DateTimeOffset LastLoginAt { get; set; }

    public ICollection<Guild> OwnedGuilds { get; set; } = [];
    public ICollection<Member> Memberships { get; set; } = [];
}
