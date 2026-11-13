using AspNet.Security.OAuth.Discord;
using Bangboo.Api.Common.Auth;
using Bangboo.Api.Common.Discord;
using Bangboo.Api.Common.Errors;
using Bangboo.Api.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bangboo.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Tags("Auth")]
public sealed class AuthController(ICurrentUser currentUser) : ControllerBase
{
    private const string DefaultReturnUrl = "/dashboard";

    /// <summary>Inicia o login com Discord.</summary>
    /// <remarks>
    /// Redireciona para o OAuth do Discord. Após o login, o cookie <c>bangboo.auth</c> é criado e o
    /// navegador volta para <paramref name="returnUrl"/> (apenas caminhos relativos; padrão <c>/dashboard</c>).
    /// Abra esta URL como navegação de página, não via fetch.
    /// </remarks>
    /// <param name="returnUrl">Caminho do dashboard para voltar após o login.</param>
    [HttpGet("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public IActionResult Login([FromQuery] string? returnUrl)
    {
        var redirect = !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : DefaultReturnUrl;
        return Challenge(new AuthenticationProperties { RedirectUri = redirect }, DiscordAuthenticationDefaults.AuthenticationScheme);
    }

    /// <summary>Encerra a sessão.</summary>
    /// <remarks>Remove o cookie de autenticação. Idempotente: responde 204 mesmo sem sessão.</remarks>
    [HttpPost("logout")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }

    /// <summary>Usuário logado.</summary>
    /// <remarks>Retorna o perfil do usuário da sessão atual, ou 401 se não houver sessão.</remarks>
    [HttpGet("me")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiError>(StatusCodes.Status401Unauthorized)]
    public ActionResult<UserResponse> Me() =>
        new UserResponse(
            currentUser.Id,
            currentUser.Username,
            currentUser.GlobalName,
            DiscordCdn.UserAvatar(currentUser.Id, currentUser.AvatarHash));
}
