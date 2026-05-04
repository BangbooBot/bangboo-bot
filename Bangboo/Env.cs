using System.ComponentModel.DataAnnotations;

namespace Bangboo;

public class Env
{
    [Required, MinLength(1)]
    public string DiscordApiUrl { get; set; }

    public ulong ClientId { get; set; }
    
    [Required, MinLength(1)]
    public string ClientSecret { get; set; }
    
    [Required, MinLength(1)]
    public string RedirectUrl { get; set; }
    
    [Required, MinLength(1)]
    public string RedisConnection { get; set; }
}

