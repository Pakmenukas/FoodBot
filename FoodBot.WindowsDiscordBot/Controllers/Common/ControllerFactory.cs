namespace FoodBot.WindowsDiscordBot.Controllers.Common;

public sealed class ControllerFactory(
    ServerController serverController,
    BankController bankController,
    KitchenController kitchenController)
{
    private readonly List<IController> _commands =
    [
        serverController,
        bankController,
        kitchenController
    ];

    public List<IController> Commands => _commands;
}