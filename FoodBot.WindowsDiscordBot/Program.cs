using DiscordBot;
using FoodBot.Application.Common;
using FoodBot.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using App = System.Windows.Forms.Application;

namespace FoodBot.WindowsDiscordBot;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var hostBuilder = Host.CreateDefaultBuilder();

        hostBuilder.ConfigureServices(serviceCollection =>
        {
            serviceCollection.Configure<DataOptions>(configuration);
            serviceCollection.ConfigureDiscordBot();
        });

        var host = hostBuilder.Build();

        host.Services.ApplyMigrations();

        App.EnableVisualStyles();
        App.SetCompatibleTextRenderingDefault(false);

        var bot = host.Services.GetRequiredService<DiscordBot.FoodBot>();
        var dataOptions = host.Services.GetRequiredService<IOptions<DataOptions>>();

        using var icon = new NotifyIcon();
        icon.Text = "FoodBot";
        var iconPath = Path.Combine(dataOptions.Value.DataPath, "icon.ico");
        icon.Icon = Icon.ExtractAssociatedIcon(iconPath);

        icon.ContextMenuStrip = new ContextMenuStrip();
        icon.ContextMenuStrip.Items.Add("Exit", null, (_, _) => { Stop(bot); });

        icon.Visible = true;
        Run(bot);
        icon.Visible = false;
    }

    private static void Run(DiscordBot.FoodBot bot)
    {
        bot.RunAsync();
        App.Run();
    }

    private static void Stop(DiscordBot.FoodBot bot)
    {
        bot.StopAsync();
        App.Exit();
    }
}