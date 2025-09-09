using DiscordBot;
using FoodBot.Application.Common;
using FoodBot.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var serviceCollection = new ServiceCollection();

serviceCollection.Configure<DataOptions>(configuration);
serviceCollection.ConfigureDiscordBot();

var serviceProvider = serviceCollection.BuildServiceProvider();

serviceProvider.ApplyMigrations();

var bot = serviceProvider.GetRequiredService<DiscordBot.FoodBot>();

await bot.RunAsync();