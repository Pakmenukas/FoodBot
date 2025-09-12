using Discord;
using Discord.WebSocket;

namespace DiscordBot.Controllers.Common;

public interface IController
{
    public List<ApplicationCommandProperties> GetCommands();
    public Task SlashCommandHandler(SocketSlashCommand command, DiscordSocketClient client);
}