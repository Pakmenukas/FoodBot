using FoodBot.Application.Common;
using Microsoft.Extensions.DependencyInjection;

namespace FoodBot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDiscordToken, DiscordTokenProvider>();
        services.AddSingleton<IMainContext, MainContext>();
        services.AddScoped<IDiscord, DiscordService.DiscordService>();

        return services;
    }
}