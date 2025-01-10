using Discord;
using Discord.WebSocket;
using FoodBot.Application.Common;
using FoodBot.Application.Kitchen;
using FoodBot.Application.Server;
using FoodBot.Domain;
using FoodBot.WindowsDiscordBot.Controllers.Common;
using FoodBot.WindowsDiscordBot.Utils;
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

        command = new SlashCommandBuilder()
            .WithName(CommandNames.Order)
            .WithDescription("Užsakyti maistą")
            .AddOption("ka", ApplicationCommandOptionType.String, "Patiekalas", isRequired: true)
            .AddOption("suma", ApplicationCommandOptionType.String, "Kaina", isRequired: true);
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.OrderMoney)
            .WithDescription("Pakeisti užsakymo kainą")
            .AddOption("suma", ApplicationCommandOptionType.String, "Kaina", isRequired: true);
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.OrderCancel)
            .WithDescription("Atsisakyti užsakymo");
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.OrdersGet)
            .WithDescription("Gauti sarašą");
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.OrdersMagicGet)
            .WithDescription("Gauti sarašą rikiuotą");
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.OrdersDone)
            .WithDescription("Užbaigti užsakyma ir rasti atsitiktinį");
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.OrdersRemoveDelivery)
            .WithDescription("Nyuskaityto už atvežimą")
            .AddOption("suma", ApplicationCommandOptionType.String, "Suma", isRequired: true);
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.Random)
            .WithDescription("Atsitiktinis šiukšlių nešėjes");
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.Kfc)
            .WithDescription("Kfc maisto užsakymo test")
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("astrumas")
                .WithDescription("ką valgysi tu")
                .WithRequired(true)
                .AddChoice("original/neaštrus", "original")
                .AddChoice("aštrus", "aštrus")
                .WithType(ApplicationCommandOptionType.String)
            )
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("gerimas")
                .WithDescription("ką gersi tu")
                .WithRequired(true)
                .AddChoice("cola", "cola")
                .AddChoice("cola_zero", "cola_zero")
                .AddChoice("fanta", "fanta")
                .AddChoice("sprite", "sprite")
                .WithType(ApplicationCommandOptionType.String)
            );
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
            case CommandNames.Order:
                await OrderCommand(command);
                break;
            case CommandNames.OrderMoney:
                await OrderMoneyCommand(command);
                break;
            case CommandNames.OrderCancel:
                await OrderCancelCommand(command);
                break;
            case CommandNames.OrdersGet:
                await OrdersGetCommand(command);
                break;
            case CommandNames.OrdersMagicGet:
                await OrdersMagicGetCommand(command);
                break;
            case CommandNames.OrdersDone:
                await OrdersDoneCommand(command);
                break;
            case CommandNames.OrdersRemoveDelivery:
                await OrdersRemoveDeliveryCommand(command);
                break;
            case CommandNames.Random:
                await RandomCommand(command);
                break;
            case CommandNames.Kfc:
                await KfcCommand(command);
                break;
        }
    }

    private async Task DrinkCommand(SocketSlashCommand command)
    {
        var result = await mediator.Send(new PurchaseDrinkCommand(command.User.Id));

        if (result.IsFailure)
        {
            await command.RespondAsync(result.Error.Description);
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

    private async Task KfcCommand(SocketSlashCommand command)
    {
        var taste = command.Data.Options.First(o => o.Name == "astrumas").Value.ToString();
        var drink = command.Data.Options.First(o => o.Name == "gerimas").Value.ToString();

        var result = await mediator.Send(new KfcCommand(command.User.Id, taste!, drink!));

        if (result.IsFailure)
        {
            await command.RespondAsync(result.Error.Description);
            return;
        }

        await command.RespondAsync($"<@{command.User.Id}> {taste} akcijinis su {drink}");
    }

    private async Task OrderCommand(SocketSlashCommand command)
    {
        var product = command.Data.Options.First().Value.ToString()!.Trim();
        var moneyObject = command.Data.Options.Skip(1).First().Value;
        var amount = MoneySerializer.Deserialize(moneyObject);
        if (amount.IsFailure)
        {
            await mediator.Send(new LogClientErrorCommand(command.User.Id, nameof(OrderCommand), amount.Error));
            await command.RespondAsync($":exclamation:Blogas skaičius **{moneyObject}**:exclamation:");
            return;
        }

        var result = await mediator.Send(new OrderCommand(command.User.Id, product, amount.Value));

        if (result.IsFailure)
        {
            await command.RespondAsync(result.Error.Description);
            return;
        }

        await command.RespondAsync($"{product} **{(float)amount.Value / 100:#0.00}** {KitchenUtils.GetRandomFood()}");
    }

    private async Task OrderCancelCommand(SocketSlashCommand command)
    {
        var result = await mediator.Send(new OrderCancelCommand(command.User.Id));

        if (result.IsFailure)
        {
            await command.RespondAsync(result.Error.Description);
            return;
        }

        await command.RespondAsync("Užsakymas pašalintas :no_mouth:");
    }

    private async Task OrderMoneyCommand(SocketSlashCommand command)
    {
        var moneyObject = command.Data.Options.First().Value;
        var amount = MoneySerializer.Deserialize(moneyObject);
        if (amount.IsFailure)
        {
            await mediator.Send(new LogClientErrorCommand(command.User.Id, nameof(OrderCommand), amount.Error));
            await command.RespondAsync($":exclamation:Blogas skaičius **{moneyObject}**:exclamation:");
            return;
        }

        var result = await mediator.Send(new OrderMoneyCommand(command.User.Id, amount.Value));

        if (result.IsFailure)
        {
            await command.RespondAsync(result.Error.Description);
            return;
        }

        await command.RespondAsync($"{result.Value.Product} **{(float)result.Value.Amount / 100:#0.00}** {KitchenUtils.GetRandomFood()}");
    }

    private async Task OrdersGetCommand(SocketSlashCommand command)
    {
        var result = await mediator.Send(new OrdersGetQuery(command.User.Id));

        if (result.IsFailure)
        {
            await command.RespondAsync(result.Error.Description);
            return;
        }

        var purchaseList = result.Value.PurchaseList;
        var sum = purchaseList.Sum(e => e.ChanceInt);

        string txt = "";
        for (int i = 0; i < purchaseList.Count; i++)
        {
            Purchase p = purchaseList[i];
            txt += $"<@{p.User.DiscordId}> {p.Product} \t **{(float)p.Money / 100:#0.00}** \t ({(p.ChanceInt / (float)sum):P1}; {p.ChanceInt}/{sum})\r\n";
        }
        txt += $"Total: **{(float)purchaseList.Sum(e => e.Money) / 100:#0.00}**";

        await command.RespondAsync(txt);
    }

    private async Task OrdersMagicGetCommand(SocketSlashCommand command)
    {
        var result = await mediator.Send(new OrdersGetQuery(command.User.Id));

        if (result.IsFailure)
        {
            await command.RespondAsync(result.Error.Description);
            return;
        }

        var purchaseList = result.Value.PurchaseList;
        var sum = purchaseList.Sum(e => e.ChanceInt);
        purchaseList.LevenshteinOrder();

        string txt = "";
        for (int i = 0; i < purchaseList.Count; i++)
        {
            Purchase p = purchaseList[i];
            txt += $"<@{p.User.DiscordId}> {p.Product} \t **{(float)p.Money / 100:#0.00}** \t ({(p.ChanceInt / (float)sum):P1}; {p.ChanceInt}/{sum})\r\n";
        }
        txt += $"Total: **{(float)purchaseList.Sum(e => e.Money) / 100:#0.00}**";

        await command.RespondAsync(txt);
    }

    private async Task OrdersDoneCommand(SocketSlashCommand command)
    {
        var result = await mediator.Send(new OrdersDoneCommand(command.User.Id));

        if (result.IsFailure)
        {
            await command.RespondAsync(result.Error.Description);
            return;
        }

        string text = "";
        text += $"################################\r\n";
        text += $"Random number: **{result.Value.Random}**/{result.Value.Sum}\r\n";
        text += $"Chosen one: <@{result.Value.GarbagePerson.DiscordId}> :game_die:\r\n";
        text += $"################################\r\n";
        int chanceCount = 0;
        foreach (var p in result.Value.PurchaseList)
        {
            text += $"<@{p.User.DiscordId}> \t {(float)(p.User.Money + p.Money) / 100:#0.00}->**{(float)(p.User.Money) / 100:#0.00}** \t";
            text += $" [{chanceCount}, {chanceCount + p.ChanceInt}) \r\n";
            chanceCount += p.ChanceInt;
        }
        text += $"################################";

        await command.RespondAsync(text);
    }


    private async Task RandomCommand(SocketSlashCommand command)
    {
        var result = await mediator.Send(new RandomUserQuery(command.User.Id));

        if (result.IsFailure)
        {
            await command.RespondAsync(result.Error.Description);
            return;
        }

        string text = "";
        if (result.Value.IsComplete)
        {
            text += "Iš pabaigto užsakymo";
        }
        else
        {
            text += "Iš nebaigto užsakymo";
        }
        ulong id = result.Value.randomUser.DiscordId;

        await command.RespondAsync($"{text}: <@{id}> :game_die:");
    }

    private async Task OrdersRemoveDeliveryCommand(SocketSlashCommand command)
    {
        var moneyObject = command.Data.Options.First().Value;
        var amount = MoneySerializer.Deserialize(moneyObject);
        if (amount.IsFailure)
        {
            await mediator.Send(new LogClientErrorCommand(command.User.Id, nameof(OrderCommand), amount.Error));
            await command.RespondAsync($":exclamation:Blogas skaičius **{moneyObject}**:exclamation:");
            return;
        }

        var result = await mediator.Send(new OrdersRemoveDeliveryCommand(command.User.Id, amount.Value));

        if (result.IsFailure)
        {
            await command.RespondAsync(result.Error.Description);
            return;
        }

        string text = "";
        text += $"Nuskaitoma (paskutinis užsakymas): **{(float)amount.Value / 100:#0.00}**, po: **{(float)result.Value.amountDividedByUsers / 100:#0.00}**\r\n";
        text += $"Dabartiniai likučiai:";

        foreach (var item in result.Value.PurchaseList)
        {
            text += $"\r\n<@{item.User.DiscordId}> \t **{(float)item.User.Money / 100:#0.00}**";
        }

        await command.RespondAsync($"{text}");
    }

}