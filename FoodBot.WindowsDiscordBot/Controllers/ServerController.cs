using Discord;
using Discord.WebSocket;
using FoodBot.WindowsDiscordBot.Controllers.Common;

namespace FoodBot.WindowsDiscordBot.Controllers;

public sealed class ServerController : IController
{
    public List<ApplicationCommandProperties> GetCommands()
    {
        List<ApplicationCommandProperties> commandList = [];
        SlashCommandBuilder command;

        command = new SlashCommandBuilder()
            .WithName(CommandNames.Ping)
            .WithDescription("Versija");
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.Changelog)
            .WithDescription("Pakeitimai");
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.Debug)
            .WithDescription("Debug");
        commandList.Add(command.Build());

        return commandList;
    }

    public async Task SlashCommandHandler(SocketSlashCommand command, DiscordSocketClient client)
    {
        switch (command.Data.Name)
        {
            case CommandNames.Ping:
                await command.RespondAsync("Pong! I'm v7");
                break;
            case CommandNames.Changelog:
                await command.RespondAsync(await File.ReadAllTextAsync("Data/changelog.txt"));
                break;
            case CommandNames.Debug:
                await command.RespondAsync($":exclamation:{command.User.GlobalName}:exclamation:");
                break;
        }
    }
}