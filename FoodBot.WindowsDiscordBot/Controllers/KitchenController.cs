using Discord;
using Discord.WebSocket;
using FoodBot.Application.Common;
using FoodBot.Application.Errors;
using FoodBot.Application.Kitchen;
using FoodBot.WindowsDiscordBot.Controllers.Common;
using MediatR;

namespace FoodBot.WindowsDiscordBot.Controllers;

public class KitchenController(ISender mediator) : IController
{
    public List<ApplicationCommandProperties> GetCommands()
    {
        List<ApplicationCommandProperties> commandList = [];
        SlashCommandBuilder command;

        command = new SlashCommandBuilder()
            .WithName(CommandNames.Idrink)
            .WithDescription("Mokėjimas už gėrimą");
        commandList.Add(command.Build());
        
        return commandList;
    }

    public async Task SlashCommandHandler(SocketSlashCommand command)
    {
        switch (command.Data.Name)
        {
            case CommandNames.Idrink:
                await DrinkCommand(command);
                break;
        }
    }
    
    private async Task DrinkCommand(SocketSlashCommand command)
    {
        var result = await mediator.Send(new PurchaseDrinkCommand(command.User.Id));

        if (result.IsFailure)
        {
            switch (result.Error)
            {
                case ForbiddenError error:
                    await command.RespondAsync(":exclamation:Per mažai turima pinigo:exclamation:");
                    break;
                case NotFoundError:
                    await command.RespondAsync(":exclamation:Nėra iš kurio vedama, arba kuriam pervedama:exclamation:");
                    break;
            }

            return;
        }

        var price = MoneySerializer.Serialize(result.Value.Amount);
        var initiatorOldMoney = MoneySerializer.Serialize(result.Value.InitiatorOldAmount);
        var initiatorNewMoney = MoneySerializer.Serialize(result.Value.InitiatorNewAmount);
        var commandOwnerOldMoney = MoneySerializer.Serialize(result.Value.CommandOwnerOldAmount);
        var commandOwnerNewMoney = MoneySerializer.Serialize(result.Value.CommandOwnerNewAmount);

        await command.RespondAsync(
            $"""
             Gėrimas **{price}** ({result.Value.Count})
             <@{command.User.Id}>: buvo **{initiatorOldMoney}**, dabar **{initiatorNewMoney}**
             <@{result.Value.OwnerDiscordId}>: buvo **{commandOwnerOldMoney}**, dabar **{commandOwnerNewMoney}**
             """
        );
    }
}