using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FoodBot.Application;
using FoodBot.Infrastructure;
using FoodBot.WindowsDiscordBot.Controllers.Common;
using FoodBot.WindowsDiscordBot.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using App = System.Windows.Forms.Application;

namespace FoodBot.WindowsDiscordBot;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var host = CreateHostBuilder().Build();

        host.Services.ApplyMigrations();

        App.EnableVisualStyles();
        App.SetCompatibleTextRenderingDefault(false);

        var bot = host.Services.GetRequiredService<DiscordBot>();

        using var icon = new NotifyIcon();
        icon.Text = "FoodBot";
        icon.Icon = Icon.ExtractAssociatedIcon($"{Path.GetDirectoryName(App.ExecutablePath)}\\Data\\icon.ico");

        icon.ContextMenuStrip = new ContextMenuStrip();
        icon.ContextMenuStrip.Items.Add("Exit", null, (_, _) => { Stop(bot); });

        icon.Visible = true;
        Run(bot);
        icon.Visible = false;
    }

    private static void Run(DiscordBot bot)
    {
        bot.RunAsync();
        App.Run();
    }

    private static void Stop(DiscordBot bot)
    {
        bot.StopAsync();
        App.Exit();
    }

    private static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.ConfigureApplication();
        services.ConfigureInfrastructure();

        services.AddControllers();
        services.AddSingleton<ControllerFactory>();

        services.AddSingleton<DiscordBot>();
        services.AddSingleton<CommandService>();
        services.AddSingleton<DiscordSocketClient>(provider =>
        {
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers | GatewayIntents.GuildMessages,
                AlwaysDownloadUsers = true
            };
            return new DiscordSocketClient(config);
        });
        services.AddTransient<BotLauncherForm>();

        return services;
    }

    private static IServiceCollection AddControllers(this IServiceCollection services)
    {
        var controllers = typeof(IController).Assembly.GetTypes()
            .Where(t => t.GetInterface(nameof(IController)) == typeof(IController))
            .ToList();
        foreach (var controller in controllers)
        {
            services.AddSingleton(controller);
        }

        return services;
    }

    private static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) => { services.ConfigureServices(); });
    }
}