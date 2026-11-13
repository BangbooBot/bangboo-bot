namespace Bangboo.Api.Common.Discord;

/// <summary>Regras de permissão do Discord usadas pelo dashboard.</summary>
public static class DiscordPermissions
{
    private const ulong Administrator = 1UL << 3;
    private const ulong ManageGuild = 1UL << 5;

    /// <summary>O usuário pode gerenciar a guild no dashboard (dono, Administrator ou Manage Server).</summary>
    public static bool CanManage(DiscordPartialGuild guild)
    {
        if (guild.Owner)
        {
            return true;
        }

        return ulong.TryParse(guild.Permissions, out var permissions)
               && (permissions & (Administrator | ManageGuild)) != 0;
    }
}
