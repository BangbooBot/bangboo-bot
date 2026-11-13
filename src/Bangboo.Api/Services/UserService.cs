using Bangboo.Api.Common.Errors;
using Bangboo.Core.Data;
using Bangboo.Core.Entities;

using Bangboo.Api.Services.Interfaces;

using Bangboo.Api.Dtos;

namespace Bangboo.Api.Services;

public sealed class UserService(AppDbContext db, TimeProvider clock) : IUserService
{
    public async Task UpsertAsync(DiscordProfile profile, CancellationToken ct)
    {
        if (!ulong.TryParse(profile.Id, out var id))
        {
            throw new ValidationException("Invalid Discord user id");
        }

        var user = await db.Users.FindAsync([id], ct);
        if (user is null)
        {
            user = new User { Id = id, Username = profile.Username };
            db.Users.Add(user);
        }

        user.Username = profile.Username;
        user.GlobalName = profile.GlobalName;
        user.Avatar = profile.AvatarHash;
        user.LastLoginAt = clock.GetUtcNow();

        await db.SaveChangesAsync(ct);
    }
}
