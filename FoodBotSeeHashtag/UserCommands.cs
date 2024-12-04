using DB;
using DB.Data;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Windows.Forms.Design.Behavior;

namespace FoodBotSeeHashtag;

public class UserCommands
{
    private MainContext _context { get => Program.context; }

    public List<ApplicationCommandProperties> GetCommands()
    {
        List<ApplicationCommandProperties> commandList = new List<ApplicationCommandProperties>();

        var command = new SlashCommandBuilder()
            .WithName(CommandNames.PING)
            .WithDescription("Versija");
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.ADD)
            .WithDescription("Pridėti pinigų")
            .AddOption("kam", ApplicationCommandOptionType.User, "Vartotojui bus pridėta tiek pinigų", isRequired: true)
            .AddOption("kiek", ApplicationCommandOptionType.String, "Pinigų suma", isRequired: true);
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.REMOVE)
            .WithDescription("Pašalinti pinigų")
            .AddOption("kiek", ApplicationCommandOptionType.String, "Pinigų suma", isRequired: true);
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.TRANSFER)
            .WithDescription("Pervesti pinigų")
            .AddOption("kam", ApplicationCommandOptionType.User, "Vartotojui kuriam bus pervesta", isRequired: true)
            .AddOption("kiek", ApplicationCommandOptionType.String, "Pinigų suma", isRequired: true);
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.BALANCE)
            .WithDescription("Pinigų likutis");
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.BALANCE_ALL)
            .WithDescription("Visų žmonių turimas pinigų kiekis");
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.ORDER)
            .WithDescription("Užsakyti maistą")
            .AddOption("ka", ApplicationCommandOptionType.String, "Patiekalas", isRequired: true)
            .AddOption("suma", ApplicationCommandOptionType.String, "Kaina", isRequired: true);
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.ORDER_MONEY)
            .WithDescription("Pakeisti užsakymo kainą")
            .AddOption("suma", ApplicationCommandOptionType.String, "Kaina", isRequired: true);
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.ORDER_CANCEL)
            .WithDescription("Atsisakyti užsakymo");
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.ORDERS_GET)
            .WithDescription("Gauti sarašą");
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.ORDERS_MAGIC_GET)
            .WithDescription("Gauti sarašą rikiuotą");
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.ORDERS_DONE)
            .WithDescription("Užbaigti užsakyma ir rasti atsitiktinį");
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.ORDERS_REMOVE_DELIVERY)
            .WithDescription("Nyuskaityto už atvežimą")
            .AddOption("suma", ApplicationCommandOptionType.String, "Suma", isRequired: true);
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.RANDOM)
            .WithDescription("Atsitiktinis šiukšlių nešėjes");
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.DEBUG)
            .WithDescription("Debug");
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.IDRINK)
            .WithDescription("Mokėjimas už gėrimą");
        commandList.Add(command.Build());

        command = new SlashCommandBuilder()
            .WithName(CommandNames.KFC)
            .WithDescription("Kfc maisto užsakymo test")
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("aštrumas")
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
            case CommandNames.PING:
                await PingCommand(command);
                break;
            case CommandNames.ADD:
                await AddCommand(command);
                break;
            case CommandNames.REMOVE:
                await RemoveCommand(command);
                break;
            case CommandNames.TRANSFER:
                await TransferCommand(command);
                break;
            case CommandNames.BALANCE:
                await BalanceCommand(command);
                break;
            case CommandNames.BALANCE_ALL:
                await BalanceAllCommand(command);
                break;
            case CommandNames.ORDER:
                await OrderCommand(command);
                break;
            case CommandNames.ORDER_MONEY:
                await OrderMoneyCommand(command);
                break;
            case CommandNames.ORDER_CANCEL:
                await OrderCancelCommand(command);
                break;
            case CommandNames.ORDERS_GET:
                await OrdersGetCommand(command);
                break;
            case CommandNames.ORDERS_MAGIC_GET:
                await OrdersMagicGetCommand(command);
                break;
            case CommandNames.ORDERS_DONE:
                await OrdersDoneCommand(command);
                break;
            case CommandNames.ORDERS_REMOVE_DELIVERY:
                await OrdersRemoveDeliveryCommand(command);
                break;
            case CommandNames.RANDOM:
                await RandomCommand(command);
                break;
            case CommandNames.DEBUG:
                await DebugCommand(command);
                break;
            case CommandNames.IDRINK:
                await DrinkCommand(command);
                break;
            case CommandNames.KFC:
                await KfcCommand(command);
                break;
        }

    }

    private async Task DrinkCommand(SocketSlashCommand command)
    {
        var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        var cmd = _context.Commands
            .Include(e => e.ToUser)
            .First(e => e.Name == "idrink");

        //validation
        if (cmd.Money > 5000)
        {
            await command.RespondAsync($":exclamation:Per daug pinigų **{cmd.Money}**:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (initiator is null)
        {
            await command.RespondAsync(":exclamation:Nėra iš kurio vedama, arba kuriam pervedama:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (initiator.Money < -20000)
        {
            await command.RespondAsync(":exclamation:Per mažai turima pinigo:exclamation:");
            _context.Log(command, false);
            return;
        }

        //db
        int transfer = cmd.Money;
        initiator.Money -= transfer;
        cmd.ToUser.Money += transfer;
        cmd.Count++;
        _context.SaveChanges();

        // display
        float ammount = ((float)transfer) / 100;

        float fromOld = ((float)initiator.Money + transfer) / 100;
        float fromNew = ((float)initiator.Money) / 100;

        float toOld = ((float)cmd.ToUser.Money - transfer) / 100;
        float toNew = ((float)cmd.ToUser.Money) / 100;

        await command.RespondAsync($@"Gėrimas **{ammount:#0.00}** ({cmd.Count})
<@{initiator.DiscordId}>: buvo **{fromOld:#0.00}**, dabar **{fromNew:#0.00}**
<@{cmd.ToUser.DiscordId}>: buvo **{toOld:#0.00}**, dabar **{toNew:#0.00}**");

        _context.Log(command, true);
    }

    private async Task KfcCommand(SocketSlashCommand command)
    {
        var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        var _taste = command.Data.Options.First().Value as string;
        var _drink = command.Data.Options.Skip(1).First().Value as string;

        // validation
        if (initiator is null)
        {
            await command.RespondAsync($":exclamation:Nėra tokio vartotojo:exclamation:");
            _context.Log(command, false);
            return;
        }

        // db
        AddOrder(initiator, $"akcijinis {_taste} {_drink}", 0);
        

        await command.RespondAsync($"WORK IN PROGRES: <@{initiator.DiscordId}> akcijinis {_taste} {_drink}");
        //_context.Log(command, true);
    }

    private void AddOrder(DB.Data.User initiator, string _product, int _money)
    {
        // db
        var lastIncomplete = _context.Orders
            .Include(e => e.PurchaseList)
            .Where(e => !e.IsComplete)
            .OrderByDescending(e => e.DateCreated)
            .FirstOrDefault();
        if (lastIncomplete is null)
        {
            lastIncomplete = new DB.Data.Order
            {
                DateCreated = DateTime.Now,
                IsComplete = false,
            };
            _context.Orders.Add(lastIncomplete);
            _context.SaveChanges();
        }
        var existing = lastIncomplete.PurchaseList.FirstOrDefault(e => e.User == initiator);
        if (existing is null)
        {
            existing = new DB.Data.Purchase();
            existing.User = initiator;
            existing.Order = lastIncomplete;
            _context.Purchases.Add(existing);
        }
        existing.Money = _money;
        existing.Product = _product;
        existing.Date = DateTime.Now;
        try
        {
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private async Task AddCommand(SocketSlashCommand command)
    {
        var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        var editUser = _context.GetUserFromCommand(command.Data.Options.First().Value);
        var _userTo = command.Data.Options.First().Value as SocketUser;
        var _moneyObj = command.Data.Options.Skip(1).First().Value;
        var _money = 0;

        // validation
        if (!IsMod(initiator))
        {
            await command.RespondAsync($":exclamation:Ne administratorius:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (!GetMoney(_moneyObj, ref _money))
        {
            await command.RespondAsync($":exclamation:Blogas skaičius **{_moneyObj}**:exclamation:");
            _context.Log(command, false);
            return;
        }

        // db
        if (editUser == null)
        {
            editUser = new DB.Data.User
            {
                DiscordId = _userTo.Id,
                Name = command.User.GlobalName,
                Role = DB.Data.Enums.Role.None,
                Money = 0,
            };
            _context.Users.Add(editUser);
        }

        editUser.Money += _money;
        editUser.Name = _userTo.GlobalName;
        _context.SaveChanges();

        // display
        float added = ((float)_money) / 100;
        float total = ((float)editUser.Money) / 100;
        await command.RespondAsync($"<@{editUser.DiscordId}>: pridėta **{added:#0.00}**, viso **{total:#0.00}**");
        _context.Log(command, true);
    }

    private async Task RemoveCommand(SocketSlashCommand command)
    {
        var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        var _moneyObj = command.Data.Options.First().Value;
        var _money = 0;

        // validation
        if (!GetMoney(_moneyObj, ref _money) || _money < 0)
        {
            await command.RespondAsync($":exclamation:Blogas skaičius **{_moneyObj}**:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (initiator is null)
        {
            await command.RespondAsync(":exclamation:Nėra tokio vartotojo:exclamation:");
            _context.Log(command, false);
            return;
        }

        // db
        initiator.Money -= _money;
        _context.SaveChanges();

        // display
        float removed = ((float)_money) / 100;
        float total = ((float)initiator.Money) / 100;
        await command.RespondAsync($"<@{initiator.DiscordId}>: pašalinta **{removed:#0.00}**, viso **{total:#0.00}**");
        _context.Log(command, true);
    }

    private async Task TransferCommand(SocketSlashCommand command)
    {
        var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        var toUser = _context.GetUserFromCommand(command.Data.Options.First().Value);
        var _moneyObj = command.Data.Options.Skip(1).First().Value;
        var _money = 0;

        // validation
        if (!GetMoney(_moneyObj, ref _money) || _money < 0)
        {
            await command.RespondAsync($":exclamation:Blogas skaičius **{_moneyObj}**:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (_money > 10000)
        {
            await command.RespondAsync($":exclamation:Per daug pinigų **{_moneyObj}**:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (initiator is null || toUser is null)
        {
            await command.RespondAsync(":exclamation:Nėra iš kurio vedama, arba kuriam pervedama:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (toUser == initiator)
        {
            await command.RespondAsync(":exclamation:Negalima sau perversti pinigu:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (initiator.Money < -20000)
        {
            await command.RespondAsync(":exclamation:Per mažai turima pinigo:exclamation:");
            _context.Log(command, false);
            return;
        }

        // db
        initiator.Money -= _money;
        toUser.Money += _money;

        // display
        float ammount = ((float)_money) / 100;

        float fromOld = ((float)initiator.Money + _money) / 100;
        float fromNew = ((float)initiator.Money) / 100;

        float toOld = ((float)toUser.Money - _money) / 100;
        float toNew = ((float)toUser.Money) / 100;

        await command.RespondAsync($@"Kiekis **{ammount:#0.00}**
<@{initiator.DiscordId}>: buvo **{fromOld:#0.00}**, dabar **{fromNew:#0.00}**
<@{toUser.DiscordId}>: buvo **{toOld:#0.00}**, dabar **{toNew:#0.00}**");
        _context.Log(command, true);
    }

    private async Task PingCommand(SocketSlashCommand command)
    {
        await command.RespondAsync($@"Pong! 
v7: 2024-12-02
- added magic order ordering
- started implementing kfc command
- updated to .net 9
v6: 2024-09-18
- increased change for debtors
v5: 2024-08-30
- fixed balance
v4: 2024-08-24
- changed database to SQLite
- transfer limit to 100
- updated random logic values
- orders get display more stats
- order done display chance and money left
- added random.org random
v3: 2024-07-10
- Fixed order done
- added idrink command
- added random command last or active order
");
    }

    private async Task BalanceCommand(SocketSlashCommand command)
    {
        var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);

        // validation
        if (initiator is null)
        {
            await command.RespondAsync(":exclamation:Nėra tokio vartotojo:exclamation:");
            _context.Log(command, false);
            return;
        }

        // display
        await command.RespondAsync($"<@{initiator.DiscordId}> iš viso: **{(float)initiator.Money / 100:#0.00}** {GetStatus(initiator.Money, 1)}");
        _context.Log(command, true);
    }

    private async Task BalanceAllCommand(SocketSlashCommand command)
    {
        var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);

        // validation
        if (!IsMod(initiator))
        {
            await command.RespondAsync($":exclamation:Ne administratorius:exclamation:");
            _context.Log(command, false);
            return;
        }

        // display
        var userOrdered = _context.Users.OrderByDescending(e => e.Money).ToList();
        var total = _context.Users.Sum(e => e.Money);

        IEnumerable<string> userTexts = userOrdered
            .Select((e, i) => $"{i}. <@{e.DiscordId}>: **{(float)e.Money / 100:#0.00}** {GetStatus(e.Money, i)}\r\n");

        string text = string.Join("", userTexts);
        text += $"Total: **{(float)total / 100:#0.00}**";

        await command.RespondAsync(text);
        _context.Log(command, true);
    }

    private async Task OrderCommand(SocketSlashCommand command)
    {
        var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        var _product = command.Data.Options.First().Value.ToString()!.Trim();
        var moneyObj = command.Data.Options.Skip(1).First().Value;
        var _money = 0;

        // validation
        if (initiator is null)
        {
            await command.RespondAsync($":exclamation:Nėra tokio vartotojo:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (!GetMoney(moneyObj, ref _money))
        {
            await command.RespondAsync($":exclamation:Blogas skaičius **{moneyObj}**:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (_money < 0)
        {
            await command.RespondAsync(":exclamation:Per maža kaina:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (_money > 10000)
        {
            await command.RespondAsync(":exclamation:Per didiėlė kaina:exclamation:");
            _context.Log(command, false);
            return;
        }

        // db
        AddOrder(initiator, _product, _money);

        // display
        await command.RespondAsync($"{_product} **{(float)_money / 100:#0.00}** {GetRandomFood()}");
        _context.Log(command, true);
    }

    private async Task OrderCancelCommand(SocketSlashCommand command)
    {
        var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        var lastIncomplete = _context.Orders
            .Include(e => e.PurchaseList)
            .Where(e => !e.IsComplete)
            .OrderByDescending(e => e.DateCreated)
            .FirstOrDefault();

        // validation
        if (initiator is null)
        {
            await command.RespondAsync($":exclamation:Nėra tokio vartotojo:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (lastIncomplete is null || !lastIncomplete.PurchaseList.Any(e => e.User == initiator))
        {
            await command.RespondAsync(":exclamation:Nėra tokio užsakymo:exclamation:");
            _context.Log(command, false);
            return;
        }

        _context.Purchases.Remove(lastIncomplete.PurchaseList.First(e => e.User == initiator));

        // display
        await command.RespondAsync("Užsakymas pašalintas :no_mouth:");
        _context.Log(command, true);
    }

    private async Task OrderMoneyCommand(SocketSlashCommand command)
    {
        var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        var lastIncomplete = _context.Orders
            .Include(e => e.PurchaseList)
            .Where(e => !e.IsComplete)
            .OrderByDescending(e => e.DateCreated)
            .FirstOrDefault();
        var moneyObj = command.Data.Options.First().Value;
        var _money = 0;

        // validation
        if (initiator is null)
        {
            await command.RespondAsync($":exclamation:Nėra tokio vartotojo:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (!GetMoney(moneyObj, ref _money))
        {
            await command.RespondAsync($":exclamation:Blogas skaičius **{moneyObj}**:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (_money > 10000)
        {
            await command.RespondAsync(":exclamation:Per didiėlė kaina:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (lastIncomplete is null || !lastIncomplete.PurchaseList.Any(e => e.User == initiator))
        {
            await command.RespondAsync(":exclamation:Nėra tokio užsakymo:exclamation:");
            _context.Log(command, false);
            return;
        }

        // db
        var purchace = lastIncomplete.PurchaseList.First(e => e.User == initiator);
        purchace.Money = _money;

        // display
        await command.RespondAsync($"{purchace.Product} **{(float)purchace.Money / 100:#0.00}** {GetRandomFood()}");
        _context.Log(command, true);
    }

    private async Task OrdersGetCommand(SocketSlashCommand command)
    {
        var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        var lastIncomplete = _context.Orders
            .Include(e => e.PurchaseList).ThenInclude(f => f.User)
            .Where(e => !e.IsComplete)
            .OrderByDescending(e => e.DateCreated)
            .FirstOrDefault();

        // validation
        if (!IsMod(initiator))
        {
            await command.RespondAsync($":exclamation:Ne administratorius:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (lastIncomplete is null)
        {
            await command.RespondAsync($":exclamation:Nėra tokio užsakymo:exclamation:");
            _context.Log(command, false);
            return;
        }

        // display
        GetChances(lastIncomplete.PurchaseList);
        var sum = lastIncomplete.PurchaseList.Sum(e => e.ChanceInt);

        string txt = "";
        for (int i = 0; i < lastIncomplete.PurchaseList.Count; i++)
        {
            DB.Data.Purchase p = lastIncomplete.PurchaseList[i];
            txt += $"<@{p.User.DiscordId}> {p.Product} \t **{(float)p.Money / 100:#0.00}** \t ({(p.ChanceInt / (float)sum):P1}; {p.ChanceInt}/{sum})\r\n";
        }
        txt += $"Total: **{(float)lastIncomplete.PurchaseList.Sum(e => e.Money) / 100:#0.00}**";

        // display
        await command.RespondAsync(txt);
        _context.Log(command, true);
    }

    private async Task OrdersMagicGetCommand(SocketSlashCommand command)
    {
        var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        var lastIncomplete = _context.Orders
            .Include(e => e.PurchaseList).ThenInclude(f => f.User)
            .Where(e => !e.IsComplete)
            .OrderByDescending(e => e.DateCreated)
            .FirstOrDefault();

        // validation
        if (!IsMod(initiator))
        {
            await command.RespondAsync($":exclamation:Ne administratorius:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (lastIncomplete is null)
        {
            await command.RespondAsync($":exclamation:Nėra tokio užsakymo:exclamation:");
            _context.Log(command, false);
            return;
        }

        // display
        GetChances(lastIncomplete.PurchaseList);
        var sum = lastIncomplete.PurchaseList.Sum(e => e.ChanceInt);

        //EmbedBuilder

        List<Purchase> purchases = lastIncomplete.PurchaseList;
        purchases.LevenshteinOrder();

        string txt = "";
        for (int i = 0; i < purchases.Count; i++)
        {
            DB.Data.Purchase p = purchases[i];
            txt += $"{p.Product} \t\t <@{p.User.DiscordId}> **{(float)p.Money / 100:#0.00}** ({(p.ChanceInt / (float)sum):P1}; {p.ChanceInt})\r\n";
        }
        txt += $"Total: **{(float)lastIncomplete.PurchaseList.Sum(e => e.Money) / 100:#0.00}**";

        // display
        await command.RespondAsync(txt);
        _context.Log(command, true);
    }

    private async Task OrdersDoneCommand(SocketSlashCommand command)
    {
        var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        var lastIncomplete = _context.Orders
            .Include(e => e.PurchaseList).ThenInclude(f => f.User)
            .Where(e => !e.IsComplete)
            .OrderByDescending(e => e.DateCreated)
            .FirstOrDefault();

        // validation
        if (!IsMod(initiator))
        {
            await command.RespondAsync($":exclamation:Ne administratorius:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (lastIncomplete is null)
        {
            await command.RespondAsync(":exclamation:Nėra tokio užsakymo:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (lastIncomplete.PurchaseList is null || lastIncomplete.PurchaseList.Count == 0)
        {
            await command.RespondAsync(":exclamation:Nėra užsakymų:exclamation:");
            _context.Log(command, false);
            return;
        }

        //db
        GetChances(lastIncomplete.PurchaseList);
        var sum = lastIncomplete.PurchaseList.Sum(e => e.ChanceInt);
        int random = GetRandomNumber(sum);

        // db removes money
        lastIncomplete.PurchaseList.ForEach(e =>
        {
            e.User.Money -= Math.Abs(e.Money);
        });
        lastIncomplete.IsComplete = true;
        lastIncomplete.DateCompleted = DateTime.Now;
        lastIncomplete.GarbagePerson = GetRandomPerson(lastIncomplete.PurchaseList, random);
        _context.SaveChanges();


        // display
        string text = "";
        text += $"################################\r\n";
        text += $"Random number: **{random}**/{sum}\r\n";
        text += $"Chosen one: <@{lastIncomplete.GarbagePerson.DiscordId}> :game_die:\r\n";
        text += $"################################\r\n";
        int chanceCount = 0;
        foreach (var p in lastIncomplete.PurchaseList)
        {
            text += $"<@{p.User.DiscordId}> \t {(float)(p.User.Money + p.Money) / 100:#0.00}->**{(float)(p.User.Money) / 100:#0.00}** \t";
            text += $" [{chanceCount}, {chanceCount + p.ChanceInt}) \r\n";
            chanceCount += p.ChanceInt;
        }
        text += $"################################";

        await command.RespondAsync(text);
        _context.Log(command, true);
    }

    private async Task RandomCommand(SocketSlashCommand command)
    {
        var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        var lastIncomplete = _context.Orders
            .Include(e => e.PurchaseList).ThenInclude(f => f.User)
            .OrderByDescending(e => e.DateCreated)
            .First();

        int randomNumber = GetRandomNumber(lastIncomplete.PurchaseList.Count); ;

        // display
        string text = "";
        if (lastIncomplete.IsComplete)
        {
            text += "Iš pabaigto užsakymo";
        }
        else
        {
            text += "Iš nebaigto užsakymo";
        }
        ulong id = lastIncomplete.PurchaseList[randomNumber].User.DiscordId;

        await command.RespondAsync($"{text}: <@{id}> :game_die:");
        _context.Log(command, true);
    }

    private async Task OrdersRemoveDeliveryCommand(SocketSlashCommand command)
    {
        var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        var lastIncomplete = _context.Orders
            .Include(e => e.PurchaseList).ThenInclude(f => f.User)
            .OrderByDescending(e => e.DateCreated)
            .First();
        var moneyObj = command.Data.Options.First().Value;
        var _money = 0;

        // validation
        if (!IsMod(initiator))
        {
            await command.RespondAsync($":exclamation:Ne administratorius:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (!GetMoney(moneyObj, ref _money))
        {
            await command.RespondAsync($":exclamation:Blogas skaičius **{moneyObj}**:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (_money > 1000)
        {
            await command.RespondAsync(":exclamation:Per didiėlė kaina:exclamation:");
            _context.Log(command, false);
            return;
        }
        if (!lastIncomplete.IsComplete)
        {
            await command.RespondAsync(":exclamation:Užsakymas dar nepabaigtas:exclamation:");
            _context.Log(command, false);
            return;
        }

        //db
        string text = "";
        int count = lastIncomplete.PurchaseList.Count;
        int ammount = (int)Math.Round(Math.Ceiling((float)_money / count));

        text += $"Nuskaitoma (paskutinis užsakymas): **{(float)_money / 100:#0.00}**, po: **{(float)ammount / 100:#0.00}**\r\n";
        text += $"Dabartiniai likučiai:";

        foreach (var p in lastIncomplete.PurchaseList)
        {
            p.User.Money -= ammount;
            text += $"\r\n<@{p.User.DiscordId}> \t **{(float)p.User.Money / 100:#0.00}**";
        }
        _context.SaveChanges();

        // display
        await command.RespondAsync($"{text}");
        _context.Log(command, true);
    }

    private async Task DebugCommand(SocketSlashCommand command)
    {
        await command.RespondAsync($":exclamation:{command.User.GlobalName}:exclamation:");
    }

    #region HELPERS
    private string GetStatus(int money, int index)
    {
        if (index == 0) return ":trophy:";
        if (money <= -2000) return ":money_with_wings: :interrobang:";
        else if (money == 0) return ":shaking_face:";
        else if (money < 0) return ":money_with_wings:";
        //else if (money <= 1000) return ":soon:";
        return "";
    }

    private bool GetMoney(object obj, ref int money)
    {
        string str = obj.ToString().Replace('.', ',');
        if (Regex.IsMatch(str, @"^[+-]?\d{1,3}([,.]\d{1,2})?$"))
        {
            money = int.Parse(Math.Round((float.Parse(str) * 100)).ToString());
            return true;
        }//4,80 *100 = 480,0003
        return false;
    }

    private string GetRandomFood()
    {
        List<string> list = new List<string>
        {
            ":carrot:",
            ":potato:",
            ":garlic:",
            ":onion:",
            ":croissant:",
            ":bagel:",
            ":bread:",
            ":french_bread:",
            ":flatbread:",
            ":pretzel:",
            ":cheese:",
            ":cooking:",
            ":pancakes:",
            ":waffle:",
            ":bacon:",
            ":cut_of_meat:",
            ":poultry_leg:",
            ":meat_on_bone:",
            ":hotdog:",
            ":hamburger:",
            ":fries:",
            ":pizza:",
            ":sandwich:",
            ":stuffed_flatbread:",
            ":taco:",
            ":burrito:",
            ":tamale:",
            ":salad:",
            ":shallow_pan_of_food:",
            ":fondue:",
            ":canned_food:",
            ":spaghetti:",
            ":ramen:",
            ":stew:",
            ":curry:",
            ":sushi:",
            ":bento:",
            ":dumpling:",
            ":oyster:",
            ":rice_ball:",
            ":rice:",
        };
        Random random = new Random();
        return list[random.Next(list.Count)];
    }

    private bool IsMod(DB.Data.User user)
    {
        return user is not null && user.Role is DB.Data.Enums.Role.Root or DB.Data.Enums.Role.Simple;
    }

    private void GetChances(List<DB.Data.Purchase> purchases)
    {
        int baseChance = 0;
        int rangeCheck = 15;

        var oderList = _context.Orders.OrderByDescending(e => e.DateCreated).Where(e => e.IsComplete);

        // get base chances
        for (int i = 0; i < purchases.Count; i++)
        {
            var p = purchases[i];

            bool garbageLast1 = oderList.Take(1).Where(e => e.GarbagePerson == p.User).Any();
            int garbageLastRange = oderList.Take(rangeCheck).Where(e => e.GarbagePerson == p.User).Count();

            int chance = 0;
            if (!garbageLast1)
            {
                chance += baseChance + ((rangeCheck - garbageLastRange) * (rangeCheck - garbageLastRange));
            }
            chance += (p.User.Money < 0 ? Math.Abs(p.User.Money) : 0);
            p.ChanceInt = chance;
        }

        // remove root no garbage persons
        purchases.ForEach(e =>
        {
            if (e.User.NoGarbage)
            {
                e.ChanceInt = 0;
            }
        });

        // check if not all zero
        if (purchases.Sum(e => e.ChanceInt) == 0)
        {
            purchases.ForEach(e =>
            {
                e.ChanceInt = 1;
            });
        }
    }

    private int GetRandomNumber(int max)
    {
        string result = "";
        using (var client = new HttpClient())
        {
            result = client.GetStringAsync($"https://www.random.org/integers/?num={1}&min={1}&max={max - 1}&col={1}&base={10}&format=plain&rnd=new").Result;
        }
        if (int.TryParse(result, out int integer))
        {
            return integer;
        }
        Random random = new Random();
        return random.Next(max);
    }

    private DB.Data.User GetRandomPerson(List<DB.Data.Purchase> purchases, int randomInt)
    {
        for (int i = 0; i < purchases.Count; i++)
        {
            randomInt -= purchases[i].ChanceInt;
            if (randomInt < 0)
            {
                return purchases[i].User;
            }
        }
        return purchases.First().User;
    }

    #endregion
}
