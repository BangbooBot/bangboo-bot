using System.Reflection;
using Bangboo;
using Bangboo.Data;
using Bangboo.Modules.Services;
using Microsoft.Extensions.Options;

public class StatusService : ServerServicesModule
{
    public StatusService(IHost host, AppDbContext dbContext, IOptions<Env> options) : base(host, dbContext, options)
    {
    }
}