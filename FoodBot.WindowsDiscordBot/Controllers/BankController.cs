using Discord;
using Discord.WebSocket;
using FoodBot.Application.Bank;
using FoodBot.Application.Common;
using FoodBot.Application.Errors;
using FoodBot.Application.Server;
using FoodBot.WindowsDiscordBot.Controllers.Common;
using FoodBot.WindowsDiscordBot.Utils;
using MediatR;

namespace FoodBot.WindowsDiscordBot.Controllers;

public class BankController(ISender mediator) : IController
{
    public List<ApplicationCommandProperties> GetCommands()
    {
        List<ApplicationCommandProperties> commandList = [];
        SlashCommandBuilder command;

        command = new SlashCommandBuilder()
            .WithName(CommandNames.Add)
            .WithDescription("Pridėti pinigų")
            .AddOption("kam", ApplicationCommandOptionType.User, "Vartotojui bus pridėta tiek pinigų", isRequired: true)
            .AddOption("kiek", ApplicationCommandOptionType.String, "Pinigų suma", isRequired: true);
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.Remove)
            .WithDescription("Pašalinti pinigų")
            .AddOption("kiek", ApplicationCommandOptionType.String, "Pinigų suma", isRequired: true);
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.Transfer)
            .WithDescription("Pervesti pinigų")
            .AddOption("kam", ApplicationCommandOptionType.User, "Vartotojui kuriam bus pervesta", isRequired: true)
            .AddOption("kiek", ApplicationCommandOptionType.String, "Pinigų suma", isRequired: true);
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.Balance)
            .WithDescription("Pinigų likutis");
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.BalanceAll)
            .WithDescription("Visų žmonių turimas pinigų kiekis");
        commandList.Add(command.Build());

        return commandList;
    }

    public async Task SlashCommandHandler(SocketSlashCommand command)
    {
        switch (command.Data.Name)
        {
            case CommandNames.Add:
                await AddCommand(command);
                break;
            case CommandNames.Remove:
                await RemoveCommand(command);
                break;
            case CommandNames.Transfer:
                await TransferCommand(command);
                break;
            case CommandNames.Balance:
                await BalanceCommand(command);
                break;
            case CommandNames.BalanceAll:
                await BalanceAllCommand(command);
                break;
        }
    }

    private async Task AddCommand(SocketSlashCommand command)
    {
        var targetUser = command.Data.Options.First().Value as SocketUser;
        var moneyObject = command.Data.Options.Skip(1).First().Value;
        var amount = MoneySerializer.Deserialize(moneyObject);
        if (amount.IsFailure)
        {
            await mediator.Send(new LogClientErrorCommand(command.User.Id, nameof(AddCommand), amount.Error));
            await command.RespondAsync($":exclamation:Blogas skaičius **{moneyObject}**:exclamation:");
            return;
        }

        var result = await mediator.Send(new AddMoneyCommand(command.User.Id,
            new AddMoneyCommand.Target(targetUser!.Id, targetUser.GlobalName),
            amount.Value));

        if (result.IsFailure)
        {
            await command.RespondAsync(result.Error.Description);
            return;
        }

        var added = MoneySerializer.Serialize(result.Value.AddedAmount);
        var total = MoneySerializer.Serialize(result.Value.NewAmount);

        await command.RespondAsync($"<@{targetUser.Id}>: pridėta **{added}**, viso **{total}**");
    }

    private async Task RemoveCommand(SocketSlashCommand command)
    {
        var moneyObject = command.Data.Options.First().Value;
        var amount = MoneySerializer.Deserialize(moneyObject);
        if (amount.IsFailure)
        {
            await mediator.Send(new LogClientErrorCommand(command.User.Id, nameof(RemoveCommand), amount.Error));
            await command.RespondAsync($":exclamation:Blogas skaičius **{moneyObject}**:exclamation:");
            return;
        }

        var result = await mediator.Send(new RemoveMoneyCommand(command.User.Id, amount.Value));

        if (result.IsFailure)
        {
            await command.RespondAsync(result.Error.Description);
            return;
        }

        var removed = MoneySerializer.Serialize(result.Value.RemovedAmount);
        var total = MoneySerializer.Serialize(result.Value.NewAmount);

        await command.RespondAsync($"<@{command.User.Id}>: pašalinta **{removed}**, viso **{total}**");
    }

    private async Task TransferCommand(SocketSlashCommand command)
    {
        var targetUser = command.Data.Options.First().Value as SocketUser;
        var moneyObject = command.Data.Options.Skip(1).First().Value;
        var amount = MoneySerializer.Deserialize(moneyObject);
        if (amount.IsFailure)
        {
            await mediator.Send(new LogClientErrorCommand(command.User.Id, nameof(AddCommand), amount.Error));
            await command.RespondAsync($":exclamation:Blogas skaičius **{moneyObject}**:exclamation:");
            return;
        }

        var result = await mediator.Send(new TransferMoneyCommand(command.User.Id, targetUser!.Id, amount.Value));

        if (result.IsFailure)
        {
            await command.RespondAsync(result.Error.Description);
            return;
        }

        var amountMoney = MoneySerializer.Serialize(result.Value.Amount);
        var initiatorOldMoney = MoneySerializer.Serialize(result.Value.InitiatorOldAmount);
        var initiatorNewMoney = MoneySerializer.Serialize(result.Value.InitiatorNewAmount);
        var targetOldMoney = MoneySerializer.Serialize(result.Value.TargetOldAmount);
        var targetNewMoney = MoneySerializer.Serialize(result.Value.TargetNewAmount);

        await command.RespondAsync(
            $"""
             Kiekis **{amountMoney}**
             <@{command.User.Id}>: buvo **{initiatorOldMoney}**, dabar **{initiatorNewMoney}**
             <@{targetUser.Id}>: buvo **{targetOldMoney}**, dabar **{targetNewMoney}**
             """
        );
    }

    private async Task BalanceCommand(SocketSlashCommand command)
    {
        var result = await mediator.Send(new GetBalanceQuery(command.User.Id));

        if (result.IsFailure)
        {
            await command.RespondAsync(result.Error.Description);
            return;
        }
        
        var amount = MoneySerializer.Serialize(result.Value);

        await command.RespondAsync(
            $"<@{command.User.Id}> iš viso: **{amount}** {UserStatusUtils.GetUserMoneyStatusEmoji(result.Value)}");
    }

    private async Task BalanceAllCommand(SocketSlashCommand command)
    {
        var result = await mediator.Send(new GetBalanceAllQuery(command.User.Id));
        if (result.IsFailure)
        {
            await command.RespondAsync(result.Error.Description);
            return;
        }

        var messageLines = result.Value.Select((x, i) =>
        {
            var amount = MoneySerializer.Serialize(x.Amount);
            var status = i == 0 ? ":trophy:" : UserStatusUtils.GetUserMoneyStatusEmoji(x.Amount);
            return $"{i}. <@{x.DiscordId}>: **{amount}** {status}";
        }).ToList();

        var total = MoneySerializer.Serialize(result.Value.Sum(x => x.Amount));
        
        messageLines.Add($"Total: **{total}**");
        
        await command.RespondAsync(string.Join('\n', messageLines));
    }
}