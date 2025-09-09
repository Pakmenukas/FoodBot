using FoodBot.Application.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FoodBot.Infrastructure;

public static class MigrateUtils
{
    public static void ApplyMigrations(this IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService<IMainContext>();
        
        Console.WriteLine("Applying migrations");
        context?.Database.EnsureCreated();

        try
        {
            context?.Database.Migrate();
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to migrate database");
            Console.WriteLine(e.Message);
        }
    }
}