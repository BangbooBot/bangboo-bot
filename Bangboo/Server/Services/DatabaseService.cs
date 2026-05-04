using Bangboo;
using Bangboo.Data;
using Bangboo.Models;
using Bangboo.Modules.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public class DatabaseService : ServerServicesModule
{
    public DatabaseService(IHost host, AppDbContext dbContext, IOptions<Env> options) : base(host, dbContext, options)
    {
    }

    public async Task<List<UsersModel>> GetUsers(ulong? id)
    {
        IQueryable<UsersModel> q = _dbContext.Users.AsQueryable();
        
        if (id is not null)
        {
            q = q.Where(u => u.Id == id);
            var filteredRes = await q.FirstAsync();

            return [filteredRes];
        }
        
        var res = await q.ToListAsync();

        return res;
    }
    
    public AppDbContext GetDbContext() => _dbContext;
}