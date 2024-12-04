using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FoodBotSeeHashtag;

public class Bot
{
    private DiscordSocketClient _client;
    private readonly CommandService _commands;
    private UserCommands _userCommands;

    public Bot()
    {
        _client = new DiscordSocketClient();
        _commands = new CommandService();
        _userCommands = new UserCommands();

        _client.Ready += ClientReadyAddSlashCommands;
        _client.SlashCommandExecuted += _userCommands.SlashCommandHandler;

        RunAsync();
    }

    private async Task ClientReadyAddSlashCommands()
    {
        List<ApplicationCommandProperties> applicationCommandProperties = _userCommands.GetCommands();
        await _client.BulkOverwriteGlobalApplicationCommandsAsync(applicationCommandProperties.ToArray());

        // to reset comamnds
        //await _client.BulkOverwriteGlobalApplicationCommandsAsync(new List<ApplicationCommandProperties>().ToArray());
    }

    private async void RunAsync()
    {
        await _client.LoginAsync(TokenType.Bot, File.ReadAllText("Data/token.txt"));
        await _client.StartAsync();
        await Task.Delay(-1);
    }

    public async void StopAsync()
    {
        await _client.StopAsync();
    }
}
