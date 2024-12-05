namespace FoodBot.WindowsDiscordBot.Controllers.Common;

public sealed class ControllerFactory(
    ServerController serverController,
    BankController bankController,
    KitchenController kitchenController,
    UserController userController)
{
    private readonly List<IController> _commands =
    [
        serverController,
        bankController,
        kitchenController,
        // TODO: delete once finished
        userController
    ];

    public List<IController> Commands => _commands;
}