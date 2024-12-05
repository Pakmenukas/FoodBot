using System.Reflection;
using FoodBot.Application.Common;
using Microsoft.Extensions.DependencyInjection;

namespace FoodBot.Application;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddSingleton<ILogger, LoggerService>();
        
        return services;
    }
}