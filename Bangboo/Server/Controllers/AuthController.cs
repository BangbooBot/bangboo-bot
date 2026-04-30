using System.Security.Cryptography;
using Bangboo.Server.DTOs;
using Bangboo.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCord.JsonModels;

namespace Bangboo.Server.Controllers;

[ApiController]
[Route("/auth")]
public class AuthController : ControllerBase
{
    private readonly DatabaseService _databaseService;
    private readonly AuthService _authService;
    
    public AuthController(DatabaseService databaseService, AuthService authService)
    {
        _databaseService = databaseService;
        _authService = authService;
    }

    /// <summary>
    /// Login user
    /// </summary>
    /// <remarks>
    /// Get user info from discord
    /// </remarks>
    [HttpGet("authorize")]
    [ProducesResponseType(typeof(AuthorizeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAuthorize()
    {
        var res = await _authService.Authorize();
        res.EnsureSuccessStatusCode();
        
        var redirectUri = res.RequestMessage?.RequestUri?.ToString();
        if (redirectUri == null)
        {
            var error = new AuthErrorResponse
            {
                Code = 500,
                Error = "Failed to authorize",
                Message = "Redirect url is null"
            };

            return StatusCode(StatusCodes.Status500InternalServerError, error);
        }
        
        return Ok(new AuthorizeResponse
        {
            Url = redirectUri
        });
    }
    
    /// <summary>
    /// Login user
    /// </summary>
    /// <remarks>
    /// Get user info from discord
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostLogin([FromBody] LoginBody body)
    {
        var tokenExchangeRes = await _authService.TokenExchange(body.Code);
        //tokenExchangeRes.EnsureSuccessStatusCode();
        
        var tokenExchangeData = await tokenExchangeRes.Content.ReadFromJsonAsync<JsonAuthToken>();
        if (tokenExchangeData == null)
        {
            var error = new AuthErrorResponse
            {
                Code = 500,
                Error = "Failed to exchange token",
                Message = "Token exchange response is null"
            };
            
            return StatusCode(StatusCodes.Status500InternalServerError, error);       
        }
        var expiresIn = DateOnly.FromDateTime(
            DateTime.UtcNow.AddSeconds(tokenExchangeData.ExpiresIn)
        );
        
        var userRes = await _authService.GetUserInfo(tokenExchangeData.AccessToken);
        var user = await userRes.Content.ReadFromJsonAsync<JsonUser>();

        if (user == null)
        {
            var error = new AuthErrorResponse
            {
                Code = 500,
                Error = "Failed to get user info",
                Message = "User info response is null"
            };
            
            return StatusCode(StatusCodes.Status500InternalServerError, error);      
        }

        var dbCtx = _databaseService.GetDbContext();
        
        var userModel = await dbCtx.Users
            .FirstOrDefaultAsync(u => u.Id == user.Id);
        if (userModel is null)
        {
            userModel = new UsersModel
            {
                Id = user.Id,
                AccessToken = tokenExchangeData.AccessToken,
                RefreshToken = tokenExchangeData.RefreshToken,
                ExpiresIn = expiresIn,
                TokenType = tokenExchangeData.TokenType,
                Scope = tokenExchangeData.Scope,
            };

            await dbCtx.Users.AddAsync(userModel);
        }
        else
        {
            userModel.AccessToken = tokenExchangeData.AccessToken;
            userModel.RefreshToken = tokenExchangeData.RefreshToken;
            userModel.ExpiresIn = expiresIn;
            userModel.TokenType = tokenExchangeData.TokenType;
            userModel.Scope = tokenExchangeData.Scope;
        }
        
        
        byte[] buffer = new byte[8];
        RandomNumberGenerator.Fill(buffer);
        ulong sessionId = BitConverter.ToUInt64(buffer, 0);
        var sessionModel = await dbCtx.Sessions
            .FirstOrDefaultAsync(s => s.Id == sessionId);
        if (sessionModel is null)
        {
            sessionModel = new SessionsModel
            {
                Id = sessionId,
                ExpiresIn = expiresIn,
                UserAgent = "",
                Platform = "",
                Language = "",
                Avatar = user.AvatarHash,
                FkUserId = user.Id,
            };
            var userDbRes = await dbCtx.Sessions.AddAsync(sessionModel);
        }
        else
        {
            sessionModel.ExpiresIn = expiresIn;
            sessionModel.UserAgent = "";
            sessionModel.Platform = "";
            sessionModel.Language = "";
            sessionModel.Avatar = $"https://cdn.discordapp.com/avatars/{user.Id}/{user.AvatarHash}.png?size=64";
            sessionModel.FkUserId = user.Id;
        }
        
        await dbCtx.SaveChangesAsync();
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddSeconds(tokenExchangeData.ExpiresIn)
        };
        HttpContext.Response.Cookies.Append("SessionId", sessionId.ToString(), cookieOptions);
        
        return Ok(new LoginResponse
        {
            Username = user.Username,
            Avatar = $"https://cdn.discordapp.com/avatars/{user.Id}/{user.AvatarHash}.png?size=64",
            ExpiresIn = expiresIn
        });
    }
    
    /// <summary>
    /// Logout user
    /// </summary>
    /// <remarks>
    /// Remove user session from database
    /// </remarks>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PostLogout()
    {
        var sessionId = 0UL;
        if (ulong.TryParse(HttpContext.Request.Cookies["SessionId"] ?? "0", out ulong result))
        {
            sessionId = result;      
            HttpContext.Response.Cookies.Delete("SessionId");
        }
        
        var dbCtx = _databaseService.GetDbContext();
        var sessionModel = await dbCtx.Sessions
            .FirstOrDefaultAsync(s => s.Id == sessionId);
        if (sessionModel is null)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }
        else
        {
            dbCtx.Sessions.Remove(sessionModel);
        }
        
        await dbCtx.SaveChangesAsync();
        
        return Ok();
    }
}