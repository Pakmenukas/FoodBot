using Discord;
using Discord.WebSocket;

namespace FoodBot.WindowsDiscordBot.Controllers.Common;

public interface IController
{
    public List<ApplicationCommandProperties> GetCommands();
    public Task SlashCommandHandler(SocketSlashCommand command);
}