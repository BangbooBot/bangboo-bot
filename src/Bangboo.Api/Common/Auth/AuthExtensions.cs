using System.Security.Claims;
using AspNet.Security.OAuth.Discord;
using Bangboo.Api.Common.Discord;
using Bangboo.Api.Common.Errors;
using Bangboo.Api.Dtos;
using Bangboo.Api.Services.Interfaces;
using Bangboo.Core.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;

namespace Bangboo.Api.Common.Auth;

public static class AuthExtensions
{
    public const string CookieName = "bangboo.auth";

    /// <summary>
    /// Cookie auth + Discord OAuth, policies e <see cref="ICurrentUser"/>.
    /// Toda rota exige login, exceto as marcadas com <c>[AllowAnonymous]</c>.
    /// </summary>
    public static IServiceCollection AddBangbooAuth(this IServiceCollection services, IHostEnvironment environment)
    {
        services.AddDataProtection()
            .SetApplicationName("Bangboo")
            .PersistKeysToDbContext<AppDbContext>();

        services.AddAuthentication(options =>
            {
                // O desafio padrão é o cookie (responde 401). O Discord só é acionado
                // explicitamente em GET api/auth/login.
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = CookieName;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = environment.IsDevelopment()
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.Always;
                // Mesma validade do access token do Discord.
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
                options.Events.OnRedirectToLogin = context =>
                    ApiErrorWriter.WriteAsync(context.HttpContext, StatusCodes.Status401Unauthorized);
                options.Events.OnRedirectToAccessDenied = context =>
                    ApiErrorWriter.WriteAsync(context.HttpContext, StatusCodes.Status403Forbidden);
            })
            .AddDiscord(options =>
            {
                options.CallbackPath = "/api/auth/callback/discord";
                options.Scope.Add("identify");
                options.Scope.Add("guilds");
                options.SaveTokens = true;
                options.ClaimActions.MapJsonKey(BangbooClaimTypes.Avatar, "avatar");
                options.ClaimActions.MapJsonKey(BangbooClaimTypes.GlobalName, "global_name");
                // Lax: o retorno do Discord é um GET top-level; None exigiria HTTPS em dev.
                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Events.OnCreatingTicket = SaveUserAsync;
                options.Events.OnRemoteFailure = context =>
                {
                    // Login cancelado/negado no Discord: volta para a home do dashboard.
                    context.Response.Redirect("/?login=failed");
                    context.HandleResponse();
                    return Task.CompletedTask;
                };
            });

        services.AddOptions<DiscordAuthenticationOptions>(DiscordAuthenticationDefaults.AuthenticationScheme)
            .Configure<IOptions<DiscordOptions>>((options, discord) =>
            {
                options.ClientId = discord.Value.ClientId;
                options.ClientSecret = discord.Value.ClientSecret;
            });

        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
            .AddPolicy(AuthPolicies.ManageGuild, policy => policy
                .RequireAuthenticatedUser()
                .AddRequirements(new ManageGuildRequirement()));

        services.AddScoped<IAuthorizationHandler, ManageGuildAuthorizationHandler>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, HttpCurrentUser>();

        // A API fica atrás do proxy do dashboard: o redirect_uri do OAuth precisa do host/esquema originais.
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto;
        });

        return services;
    }

    private static async Task SaveUserAsync(OAuthCreatingTicketContext context)
    {
        var principal = context.Principal ?? throw new UnauthorizedException("Discord login failed");
        var users = context.HttpContext.RequestServices.GetRequiredService<IUserService>();

        await users.UpsertAsync(
            new DiscordProfile(
                Id: principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedException("Discord login failed"),
                Username: principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                GlobalName: principal.FindFirstValue(BangbooClaimTypes.GlobalName),
                AvatarHash: principal.FindFirstValue(BangbooClaimTypes.Avatar)),
            context.HttpContext.RequestAborted);
    }
}
