using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using FoodBot.WindowsDiscordBot.Controllers.Common;
using MediatR;

namespace FoodBot.WindowsDiscordBot;

public class UserController(ISender mediator) : IController
{
    public List<ApplicationCommandProperties> GetCommands()
    {
        List<ApplicationCommandProperties> commandList = [];
        SlashCommandBuilder command;

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

    private async Task KfcCommand(SocketSlashCommand command)
    {
        // var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        // var _taste = command.Data.Options.First().Value as string;
        // var _drink = command.Data.Options.Skip(1).First().Value as string;
        //
        // // validation
        // if (initiator is null)
        // {
        //     await command.RespondAsync($":exclamation:Nėra tokio vartotojo:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        //
        // // db
        // AddOrder(initiator, $"akcijinis {_taste} {_drink}", 0);
        //
        //
        // await command.RespondAsync($"WORK IN PROGRES: <@{initiator.DiscordId}> akcijinis {_taste} {_drink}");
        // //_context.Log(command, true);
    }

    // private void AddOrder(DB.Data.User initiator, string _product, int _money)
    // {
        // // db
        // var lastIncomplete = _context.Orders
        //     .Include(e => e.PurchaseList)
        //     .Where(e => !e.IsComplete)
        //     .OrderByDescending(e => e.DateCreated)
        //     .FirstOrDefault();
        // if (lastIncomplete is null)
        // {
        //     lastIncomplete = new DB.Data.Order
        //     {
        //         DateCreated = DateTime.Now,
        //         IsComplete = false,
        //     };
        //     _context.Orders.Add(lastIncomplete);
        //     _context.SaveChanges();
        // }
        // var existing = lastIncomplete.PurchaseList.FirstOrDefault(e => e.User == initiator);
        // if (existing is null)
        // {
        //     existing = new DB.Data.Purchase();
        //     existing.User = initiator;
        //     existing.Order = lastIncomplete;
        //     _context.Purchases.Add(existing);
        // }
        // existing.Money = _money;
        // existing.Product = _product;
        // existing.Date = DateTime.Now;
        // try
        // {
        //     _context.SaveChanges();
        // }
        // catch (Exception ex)
        // {
        //     throw;
        // }
    // }

    private async Task OrderCommand(SocketSlashCommand command)
    {
        // var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        // var _product = command.Data.Options.First().Value.ToString()!.Trim();
        // var moneyObj = command.Data.Options.Skip(1).First().Value;
        // var _money = 0;
        //
        // // validation
        // if (initiator is null)
        // {
        //     await command.RespondAsync($":exclamation:Nėra tokio vartotojo:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        // if (!GetMoney(moneyObj, ref _money))
        // {
        //     await command.RespondAsync($":exclamation:Blogas skaičius **{moneyObj}**:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        // if (_money < 0)
        // {
        //     await command.RespondAsync(":exclamation:Per maža kaina:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        // if (_money > 10000)
        // {
        //     await command.RespondAsync(":exclamation:Per didiėlė kaina:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        //
        // // db
        // AddOrder(initiator, _product, _money);
        //
        // // display
        // await command.RespondAsync($"{_product} **{(float)_money / 100:#0.00}** {GetRandomFood()}");
        // _context.Log(command, true);
    }

    private async Task OrderCancelCommand(SocketSlashCommand command)
    {
        // var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        // var lastIncomplete = _context.Orders
        //     .Include(e => e.PurchaseList)
        //     .Where(e => !e.IsComplete)
        //     .OrderByDescending(e => e.DateCreated)
        //     .FirstOrDefault();
        //
        // // validation
        // if (initiator is null)
        // {
        //     await command.RespondAsync($":exclamation:Nėra tokio vartotojo:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        // if (lastIncomplete is null || !lastIncomplete.PurchaseList.Any(e => e.User == initiator))
        // {
        //     await command.RespondAsync(":exclamation:Nėra tokio užsakymo:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        //
        // _context.Purchases.Remove(lastIncomplete.PurchaseList.First(e => e.User == initiator));
        //
        // // display
        // await command.RespondAsync("Užsakymas pašalintas :no_mouth:");
        // _context.Log(command, true);
    }

    private async Task OrderMoneyCommand(SocketSlashCommand command)
    {
        // var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        // var lastIncomplete = _context.Orders
        //     .Include(e => e.PurchaseList)
        //     .Where(e => !e.IsComplete)
        //     .OrderByDescending(e => e.DateCreated)
        //     .FirstOrDefault();
        // var moneyObj = command.Data.Options.First().Value;
        // var _money = 0;
        //
        // // validation
        // if (initiator is null)
        // {
        //     await command.RespondAsync($":exclamation:Nėra tokio vartotojo:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        // if (!GetMoney(moneyObj, ref _money))
        // {
        //     await command.RespondAsync($":exclamation:Blogas skaičius **{moneyObj}**:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        // if (_money > 10000)
        // {
        //     await command.RespondAsync(":exclamation:Per didiėlė kaina:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        // if (lastIncomplete is null || !lastIncomplete.PurchaseList.Any(e => e.User == initiator))
        // {
        //     await command.RespondAsync(":exclamation:Nėra tokio užsakymo:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        //
        // // db
        // var purchace = lastIncomplete.PurchaseList.First(e => e.User == initiator);
        // purchace.Money = _money;
        //
        // // display
        // await command.RespondAsync($"{purchace.Product} **{(float)purchace.Money / 100:#0.00}** {GetRandomFood()}");
        // _context.Log(command, true);
    }

    private async Task OrdersGetCommand(SocketSlashCommand command)
    {
        // var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        // var lastIncomplete = _context.Orders
        //     .Include(e => e.PurchaseList).ThenInclude(f => f.User)
        //     .Where(e => !e.IsComplete)
        //     .OrderByDescending(e => e.DateCreated)
        //     .FirstOrDefault();
        //
        // // validation
        // if (!IsMod(initiator))
        // {
        //     await command.RespondAsync($":exclamation:Ne administratorius:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        // if (lastIncomplete is null)
        // {
        //     await command.RespondAsync($":exclamation:Nėra tokio užsakymo:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        //
        // // display
        // GetChances(lastIncomplete.PurchaseList);
        // var sum = lastIncomplete.PurchaseList.Sum(e => e.ChanceInt);
        //
        // string txt = "";
        // for (int i = 0; i < lastIncomplete.PurchaseList.Count; i++)
        // {
        //     DB.Data.Purchase p = lastIncomplete.PurchaseList[i];
        //     txt += $"<@{p.User.DiscordId}> {p.Product} \t **{(float)p.Money / 100:#0.00}** \t ({(p.ChanceInt / (float)sum):P1}; {p.ChanceInt}/{sum})\r\n";
        // }
        // txt += $"Total: **{(float)lastIncomplete.PurchaseList.Sum(e => e.Money) / 100:#0.00}**";
        //
        // // display
        // await command.RespondAsync(txt);
        // _context.Log(command, true);
    }

    private async Task OrdersMagicGetCommand(SocketSlashCommand command)
    {
        // var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        // var lastIncomplete = _context.Orders
        //     .Include(e => e.PurchaseList).ThenInclude(f => f.User)
        //     .Where(e => !e.IsComplete)
        //     .OrderByDescending(e => e.DateCreated)
        //     .FirstOrDefault();
        //
        // // validation
        // if (!IsMod(initiator))
        // {
        //     await command.RespondAsync($":exclamation:Ne administratorius:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        // if (lastIncomplete is null)
        // {
        //     await command.RespondAsync($":exclamation:Nėra tokio užsakymo:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        //
        // // display
        // GetChances(lastIncomplete.PurchaseList);
        // var sum = lastIncomplete.PurchaseList.Sum(e => e.ChanceInt);
        //
        // //EmbedBuilder
        //
        // List<Purchase> purchases = lastIncomplete.PurchaseList;
        // purchases.LevenshteinOrder();
        //
        // string txt = "";
        // for (int i = 0; i < purchases.Count; i++)
        // {
        //     DB.Data.Purchase p = purchases[i];
        //     txt += $"{p.Product} \t\t <@{p.User.DiscordId}> **{(float)p.Money / 100:#0.00}** ({(p.ChanceInt / (float)sum):P1}; {p.ChanceInt})\r\n";
        // }
        // txt += $"Total: **{(float)lastIncomplete.PurchaseList.Sum(e => e.Money) / 100:#0.00}**";
        //
        // // display
        // await command.RespondAsync(txt);
        // _context.Log(command, true);
    }

    private async Task OrdersDoneCommand(SocketSlashCommand command)
    {
        // var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        // var lastIncomplete = _context.Orders
        //     .Include(e => e.PurchaseList).ThenInclude(f => f.User)
        //     .Where(e => !e.IsComplete)
        //     .OrderByDescending(e => e.DateCreated)
        //     .FirstOrDefault();
        //
        // // validation
        // if (!IsMod(initiator))
        // {
        //     await command.RespondAsync($":exclamation:Ne administratorius:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        // if (lastIncomplete is null)
        // {
        //     await command.RespondAsync(":exclamation:Nėra tokio užsakymo:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        // if (lastIncomplete.PurchaseList is null || lastIncomplete.PurchaseList.Count == 0)
        // {
        //     await command.RespondAsync(":exclamation:Nėra užsakymų:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        //
        // //db
        // GetChances(lastIncomplete.PurchaseList);
        // var sum = lastIncomplete.PurchaseList.Sum(e => e.ChanceInt);
        // int random = GetRandomNumber(sum);
        //
        // // db removes money
        // lastIncomplete.PurchaseList.ForEach(e =>
        // {
        //     e.User.Money -= Math.Abs(e.Money);
        // });
        // lastIncomplete.IsComplete = true;
        // lastIncomplete.DateCompleted = DateTime.Now;
        // lastIncomplete.GarbagePerson = GetRandomPerson(lastIncomplete.PurchaseList, random);
        // _context.SaveChanges();
        //
        //
        // // display
        // string text = "";
        // text += $"################################\r\n";
        // text += $"Random number: **{random}**/{sum}\r\n";
        // text += $"Chosen one: <@{lastIncomplete.GarbagePerson.DiscordId}> :game_die:\r\n";
        // text += $"################################\r\n";
        // int chanceCount = 0;
        // foreach (var p in lastIncomplete.PurchaseList)
        // {
        //     text += $"<@{p.User.DiscordId}> \t {(float)(p.User.Money + p.Money) / 100:#0.00}->**{(float)(p.User.Money) / 100:#0.00}** \t";
        //     text += $" [{chanceCount}, {chanceCount + p.ChanceInt}) \r\n";
        //     chanceCount += p.ChanceInt;
        // }
        // text += $"################################";
        //
        // await command.RespondAsync(text);
        // _context.Log(command, true);
    }

    private async Task RandomCommand(SocketSlashCommand command)
    {
        // var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        // var lastIncomplete = _context.Orders
        //     .Include(e => e.PurchaseList).ThenInclude(f => f.User)
        //     .OrderByDescending(e => e.DateCreated)
        //     .First();
        //
        // int randomNumber = GetRandomNumber(lastIncomplete.PurchaseList.Count); ;
        //
        // // display
        // string text = "";
        // if (lastIncomplete.IsComplete)
        // {
        //     text += "Iš pabaigto užsakymo";
        // }
        // else
        // {
        //     text += "Iš nebaigto užsakymo";
        // }
        // ulong id = lastIncomplete.PurchaseList[randomNumber].User.DiscordId;
        //
        // await command.RespondAsync($"{text}: <@{id}> :game_die:");
        // _context.Log(command, true);
    }

    private async Task OrdersRemoveDeliveryCommand(SocketSlashCommand command)
    {
        // var initiator = _context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id);
        // var lastIncomplete = _context.Orders
        //     .Include(e => e.PurchaseList).ThenInclude(f => f.User)
        //     .OrderByDescending(e => e.DateCreated)
        //     .First();
        // var moneyObj = command.Data.Options.First().Value;
        // var _money = 0;
        //
        // // validation
        // if (!IsMod(initiator))
        // {
        //     await command.RespondAsync($":exclamation:Ne administratorius:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        // if (!GetMoney(moneyObj, ref _money))
        // {
        //     await command.RespondAsync($":exclamation:Blogas skaičius **{moneyObj}**:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        // if (_money > 1000)
        // {
        //     await command.RespondAsync(":exclamation:Per didiėlė kaina:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        // if (!lastIncomplete.IsComplete)
        // {
        //     await command.RespondAsync(":exclamation:Užsakymas dar nepabaigtas:exclamation:");
        //     _context.Log(command, false);
        //     return;
        // }
        //
        // //db
        // string text = "";
        // int count = lastIncomplete.PurchaseList.Count;
        // int ammount = (int)Math.Round(Math.Ceiling((float)_money / count));
        //
        // text += $"Nuskaitoma (paskutinis užsakymas): **{(float)_money / 100:#0.00}**, po: **{(float)ammount / 100:#0.00}**\r\n";
        // text += $"Dabartiniai likučiai:";
        //
        // foreach (var p in lastIncomplete.PurchaseList)
        // {
        //     p.User.Money -= ammount;
        //     text += $"\r\n<@{p.User.DiscordId}> \t **{(float)p.User.Money / 100:#0.00}**";
        // }
        // _context.SaveChanges();
        //
        // // display
        // await command.RespondAsync($"{text}");
        // _context.Log(command, true);
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

    // private void GetChances(List<DB.Data.Purchase> purchases)
    // {
    //     int baseChance = 0;
    //     int rangeCheck = 15;
    //
    //     var oderList = _context.Orders.OrderByDescending(e => e.DateCreated).Where(e => e.IsComplete);
    //
    //     // get base chances
    //     for (int i = 0; i < purchases.Count; i++)
    //     {
    //         var p = purchases[i];
    //
    //         bool garbageLast1 = oderList.Take(1).Where(e => e.GarbagePerson == p.User).Any();
    //         int garbageLastRange = oderList.Take(rangeCheck).Where(e => e.GarbagePerson == p.User).Count();
    //
    //         int chance = 0;
    //         if (!garbageLast1)
    //         {
    //             chance += baseChance + ((rangeCheck - garbageLastRange) * (rangeCheck - garbageLastRange));
    //         }
    //         chance += (p.User.Money < 0 ? Math.Abs(p.User.Money) : 0);
    //         p.ChanceInt = chance;
    //     }
    //
    //     // remove root no garbage persons
    //     purchases.ForEach(e =>
    //     {
    //         if (e.User.NoGarbage)
    //         {
    //             e.ChanceInt = 0;
    //         }
    //     });
    //
    //     // check if not all zero
    //     if (purchases.Sum(e => e.ChanceInt) == 0)
    //     {
    //         purchases.ForEach(e =>
    //         {
    //             e.ChanceInt = 1;
    //         });
    //     }
    // }

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

    // private DB.Data.User GetRandomPerson(List<DB.Data.Purchase> purchases, int randomInt)
    // {
    //     for (int i = 0; i < purchases.Count; i++)
    //     {
    //         randomInt -= purchases[i].ChanceInt;
    //         if (randomInt < 0)
    //         {
    //             return purchases[i].User;
    //         }
    //     }
    //     return purchases.First().User;
    // }

    #endregion
}
