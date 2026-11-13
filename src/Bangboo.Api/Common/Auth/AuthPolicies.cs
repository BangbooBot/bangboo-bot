namespace Bangboo.Api.Common.Auth;

public static class AuthPolicies
{
    /// <summary>
    /// O usuário pode gerenciar a guild do parâmetro de rota <c>{guildId}</c>
    /// (dono, Administrator ou Manage Server).
    /// </summary>
    public const string ManageGuild = "ManageGuild";

    /// <summary>Nome do parâmetro de rota lido pela policy <see cref="ManageGuild"/>.</summary>
    public const string GuildIdRouteValue = "guildId";
}
