using Microsoft.Extensions.DependencyInjection;

namespace FoodBot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<MainContext>();
        
        return services;
    }
}