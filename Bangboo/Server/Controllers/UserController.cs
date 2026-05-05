using System.Security.Cryptography;
using Bangboo.Server.DTOs;
using Bangboo.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCord;
using NetCord.JsonModels;

namespace Bangboo.Server.Controllers;

[ApiController]
[Route("/user")]
public class UserController : ControllerBase
{
    private readonly DatabaseService _databaseService;
    private readonly UserService _userService;
    private readonly AuthService _authService;
    
    public UserController(DatabaseService databaseService, UserService userService, AuthService authService)
    {
        _databaseService = databaseService;
        _userService = userService;
        _authService = authService;
    }

    /// <summary>
    /// Get user info
    /// </summary>
    /// <remarks>
    /// Get user info from discord
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(JsonUser), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(String), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(String), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get()
    {
        var (result, session) = await _authService.ValidateSession(HttpContext.Request);
        if (!result)
        {
            return Unauthorized();
        }
        
        var dbCtx = _databaseService.GetDbContext();
        
        var authToken = await dbCtx.Auths.Where(a => a.Id == session.FkAuthId).GroupBy(a => a.AccessToken).FirstOrDefaultAsync();
        if (authToken is null)
        {
            return NotFound();
        }
        
        var userRes = await _userService.GetUserInfo(authToken.Key);
        if (!userRes.IsSuccessStatusCode)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, userRes.ReasonPhrase);      
        }
        var user = await userRes.Content.ReadFromJsonAsync<JsonUser>();
        if (user == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get user info");       
        }
        
        return Ok(user);
    }

    /// <summary>
    /// Disconect device from user
    /// </summary>
    /// <remarks>
    /// Disconnect device from related to user session
    /// </remarks>
    [HttpGet("guilds")]
    [ProducesResponseType(typeof(List<UserGuildResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGuilds()
    {
        var (result, session) = await _authService.ValidateSession(HttpContext.Request);
        if (!result)
        {
            return Unauthorized();       
        }

        var dbCtx = _databaseService.GetDbContext();
        
        var authToken = await dbCtx.Auths.Where(a => a.Id == session.FkAuthId).FirstOrDefaultAsync();
        if (authToken is null)
        {
            return NotFound();
        }

        var token = authToken.AccessToken;
        var guildsRes = await _userService.GetUserGuilds(token);
        if (!guildsRes.IsSuccessStatusCode)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, guildsRes.ReasonPhrase);      
        }

        var botGuilds = await _userService.Client.GetCurrentUserGuildsAsync().ToListAsync();
        
        var guilds = await guildsRes.Content.ReadFromJsonAsync<List<ApiUserGuild>>();
        var guildsOwner = guilds.Where(g =>
        {
            var permissions = g.Permissions;
            return g.Owner ||
                   (ulong.TryParse(permissions, out var value) &&
                    ((Permissions)value).HasFlag(Permissions.ManageGuild));
        }).Select(g =>
        {
            var isMember = ulong.TryParse(g.Id, out var id) && botGuilds.Exists(bg => bg.Id == id);
            return new UserGuildResponse
            {
                Id = g.Id,
                Name = g.Name,
                Icon = g.Icon,
                Owner = g.Owner,
                Permissions = g.Permissions,
                Features = g.Features,
                ApproximateMemberCount = g.ApproximateMemberCount,
                ApproximatePresenceCount = g.ApproximatePresenceCount,
                Banner = g.Banner,
                IsMember = isMember
            };
        });

        return Ok(guildsOwner);
    }
}