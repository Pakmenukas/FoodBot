using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FoodBot.Infrastructure;

public static class MigrateUtils
{
    public static void ApplyMigrations(this IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService<MainContext>();
        
        context?.Database.EnsureCreated();
        try
        {
            context?.Database.Migrate();
        }
        catch (Exception _)
        {
            // ignored
        }
    }
}