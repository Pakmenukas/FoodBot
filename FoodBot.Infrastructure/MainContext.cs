using FoodBot.Application.Common;
using FoodBot.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FoodBot.Infrastructure;

public class MainContext(IOptions<DataOptions> dataOptions) : DbContext, IMainContext
{
    // tables
    public DbSet<Log> Logs => Set<Log>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Command> Commands => Set<Command>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Purchase> Purchases => Set<Purchase>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var dbPath = Path.Combine(dataOptions.Value.DataPath, "main.db");
        options.UseSqlite($"Data Source={dbPath}");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Order>()
            .HasMany(x => x.PurchaseList)
            .WithOne(x => x.Order)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
