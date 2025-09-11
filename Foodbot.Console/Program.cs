using DiscordBot;
using FoodBot.Application.Common;
using FoodBot.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var iconPath = configuration.GetValue<string>("IconPath");
var serviceCollection = new ServiceCollection();

serviceCollection.Configure<AppOptions>(configuration);
serviceCollection.ConfigureDiscordBot();

var serviceProvider = serviceCollection.BuildServiceProvider();

serviceProvider.ApplyMigrations();

var bot = serviceProvider.GetRequiredService<DiscordBot.FoodBot>();

await bot.RunAsync();