using FoodBot.Domain;
using Microsoft.EntityFrameworkCore;

namespace FoodBot.Infrastructure;

public class MainContext : DbContext
{
    // tables
    public DbSet<Log> Logs => Set<Log>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Command> Commands => Set<Command>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Purchase> Purchases => Set<Purchase>();

    // config
    private static string DbPath => "Data\\main.db";

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
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
