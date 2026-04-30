using bangboo_backend.Data;
using Microsoft.EntityFrameworkCore;

public class DatabaseService
{
    private readonly AppDbContext _dbContext;

    public DatabaseService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
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