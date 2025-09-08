using DiscordBot;
using Microsoft.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection();

serviceCollection.ConfigureDiscordBot();

var serviceProvider = serviceCollection.BuildServiceProvider();

var bot = serviceProvider.GetRequiredService<DiscordBot.FoodBot>();

await bot.RunAsync();