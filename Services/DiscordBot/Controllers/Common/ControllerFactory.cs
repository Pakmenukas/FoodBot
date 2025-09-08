namespace DiscordBot.Controllers.Common;

public sealed class ControllerFactory(
    ServerController serverController,
    BankController bankController,
    KitchenController kitchenController)
{
    public List<IController> Commands { get; } =
    [
        serverController,
        bankController,
        kitchenController
    ];
}