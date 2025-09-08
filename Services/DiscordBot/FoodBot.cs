using Discord;
using Discord.WebSocket;
using DiscordBot.Controllers.Common;
using FoodBot.Application.Common;

namespace DiscordBot;

public sealed class FoodBot
{
    private readonly DiscordSocketClient _client;
    private readonly ControllerFactory _controllerFactory;
    private readonly IDiscordToken _tokenProvider;

    public FoodBot(DiscordSocketClient client, 
        ControllerFactory controllerFactory,
        IDiscordToken tokenProvider)
    {
        _client = client;
        _controllerFactory = controllerFactory;
        _tokenProvider = tokenProvider;

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
#if !DEBUG
        const ulong FOOD_FORTRESS_CHANNEL_ID = 598067153052106754;
        const ulong FOOD_FORTRESS_TESTGROUND_CHANNEL_ID = 1035470203385024532;

        if (command.Channel.Id != FOOD_FORTRESS_CHANNEL_ID && command.Channel.Id != FOOD_FORTRESS_TESTGROUND_CHANNEL_ID)
        {
            await command.RespondAsync($"❌ Komandas galima naudoti tik <#{FOOD_FORTRESS_CHANNEL_ID}> kanale",
                ephemeral: true);
            return;
        }
#endif

        foreach (var userCommand in _controllerFactory.Commands)
        {
            await userCommand.SlashCommandHandler(command, _client);
        }
    }

    // TODO: check if windows app accepts method with Task return type
    public async Task RunAsync()
    {
        Console.WriteLine("Starting bot...");
        await _client.LoginAsync(TokenType.Bot, _tokenProvider.Token);
        switch (_client.LoginState)
        {
            case LoginState.LoggedOut:
                Console.WriteLine("Bot logged out");
                break;
            case LoginState.LoggingIn:
                Console.WriteLine("Bot is still logging in");
                break;
            case LoginState.LoggedIn:
                Console.WriteLine("Bot logged in");
                break;
            case LoginState.LoggingOut:
                Console.WriteLine("Bot is logging out");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        await _client.StartAsync();
        Console.WriteLine("Bot started");
        await Task.Delay(-1);
    }

    public async void StopAsync()
    {
        await _client.StopAsync();
    }
}