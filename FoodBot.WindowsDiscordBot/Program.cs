using DiscordBot;
using FoodBot.Application.Common;
using FoodBot.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using App = System.Windows.Forms.Application;

namespace FoodBot.WindowsDiscordBot;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
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

        using var icon = new NotifyIcon();
        icon.Text = "FoodBot";
        icon.Icon = Icon.ExtractAssociatedIcon($"{Path.GetDirectoryName(App.ExecutablePath)}\\Data\\icon.ico");

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