using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Controllers.Common;
using FoodBot.Application;
using FoodBot.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureDiscordBot(this IServiceCollection services)
    {
        services.AddSingleton<ControllerFactory>();
        services.AddInfrastructure();
        services.AddApplication();
        var controllers = typeof(IController).Assembly.GetTypes()
            .Where(t => t.GetInterface(nameof(IController)) == typeof(IController))
            .ToList();
        foreach (var controller in controllers)
        {
            services.AddSingleton(controller);
        }
        
        services.AddSingleton<FoodBot>();
        services.AddSingleton<CommandService>();
        services.AddSingleton<DiscordSocketClient>(_ =>
        {
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers | GatewayIntents.GuildMessages,
                AlwaysDownloadUsers = true
            };
            return new DiscordSocketClient(config);
        });

        return services;
    }
}