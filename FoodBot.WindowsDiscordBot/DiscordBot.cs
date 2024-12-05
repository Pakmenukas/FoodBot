using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FoodBot.WindowsDiscordBot.Controllers.Common;

namespace FoodBot.WindowsDiscordBot;

public sealed class DiscordBot
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly ControllerFactory _controllerFactory;

    public DiscordBot(DiscordSocketClient client, CommandService commandService, ControllerFactory controllerFactory)
    {
        _client = client;
        _commands = commandService;
        _controllerFactory = controllerFactory;

        _client.SlashCommandExecuted += SlashCommandHandler;
        _client.Ready += ClientReadyAddSlashCommands;
    }
    
    private async Task ClientReadyAddSlashCommands()
    {
        var applicationCommandProperties = _controllerFactory.Commands.SelectMany(x => x.GetCommands());
        await _client.BulkOverwriteGlobalApplicationCommandsAsync(applicationCommandProperties.ToArray());
    }
    
    private async Task SlashCommandHandler(SocketSlashCommand command)
    {
        foreach (var userCommand in _controllerFactory.Commands)
        {
            await userCommand.SlashCommandHandler(command);
        }
    }

    public async void RunAsync()
    {
        await _client.LoginAsync(TokenType.Bot, await File.ReadAllTextAsync("Data/token.txt"));
        await _client.StartAsync();
        await Task.Delay(-1);
    }

    public async void StopAsync()
    {
        await _client.StopAsync();
    }
}
