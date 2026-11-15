using Bangboo.Api.Common.Discord;
using Bangboo.Core.Data;
using Bangboo.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bangboo.Api.Services;

public static class GuildUpsert
{
    /// <summary>
    /// Garante a linha da guild na tabela <c>guilds</c> (FK das configurações) e atualiza nome/ícone/banner.
    /// Não salva: quem chama faz o <c>SaveChangesAsync</c> junto com a configuração.
    /// </summary>
    public static async Task UpsertGuildAsync(this AppDbContext db, ulong id, DiscordGuild discordGuild, CancellationToken ct)
    {
        var guild = await db.Guilds.FirstOrDefaultAsync(g => g.Id == id, ct);
        if (guild is null)
        {
            guild = new Guild { Id = id, Name = discordGuild.Name };
            db.Guilds.Add(guild);
        }

        guild.Name = discordGuild.Name;
        guild.Icon = discordGuild.Icon;
        guild.Banner = discordGuild.Banner;
    }
}
