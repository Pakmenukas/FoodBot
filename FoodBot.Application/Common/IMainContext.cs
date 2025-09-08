using FoodBot.Domain;
using Microsoft.EntityFrameworkCore;

namespace FoodBot.Application.Common;

public interface IMainContext
{
    public DbSet<Log> Logs { get; }
    public DbSet<User> Users { get; }
    public DbSet<Command> Commands { get; }
    public DbSet<Order> Orders { get; }
    public DbSet<Purchase> Purchases { get; }
    
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}