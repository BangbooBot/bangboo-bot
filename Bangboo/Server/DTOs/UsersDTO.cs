using NetCord;

namespace Bangboo.Server.DTOs;

public class UserSessionResponse
{
    public ulong Id { get; set; }
    
    public string UserAgent { get; set; }
    
    public DateOnly ExpiresIn { get; set; }
    
    public string Language { get; set; }
}

public class ApiUserGuild
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Banner { get; set; }
    public bool Owner { get; set; }
    public string? Permissions { get; set; }
    public string[] Features { get; set; }
    public uint ApproximateMemberCount { get; set; }
    public uint ApproximatePresenceCount { get; set; }
}

public class UserGuildResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Banner { get; set; }
    public bool Owner { get; set; }
    public bool IsMember { get; set; }
    public string? Permissions { get; set; }
    public string[] Features { get; set; }
    public uint ApproximateMemberCount { get; set; }
    public uint ApproximatePresenceCount { get; set; }
}

