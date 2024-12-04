using DB.Data;
using Microsoft.EntityFrameworkCore;

namespace DB;

public class MainContext : DbContext
{
    // tables
    public DbSet<Log> Logs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Command> Commands { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Purchase> Purchases { get; set; }

    // config
    public string DbPath { get; }

    public MainContext()
    {
        DbPath = "Data\\main.db";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>()
            .HasMany(x => x.PurchaseList)
            .WithOne(x => x.Order)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
