using DB;
using DB.Data;
using Discord.WebSocket;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FoodBotSeeHashtag;

public static class Helpers
{
    public static void Log(this MainContext context, SocketSlashCommand command, bool success)
    {
        var log = new Log
        {
            Date = DateTime.Now,
            User = context.Users.FirstOrDefault(e => e.DiscordId == command.User.Id),
            Success = success,
            Command = command.Data.Name,
            Data = string.Join("|", command.Data.Options.Select(e => e.Value.ToString()!.Replace('|', '/'))),
        };
        context.Logs.Add(log);
        context.SaveChanges();
    }

    public static DB.Data.User GetUserFromCommand(this MainContext context, object command)
    {
        if (command is SocketUser socketUser)
        {
            return context.Users.FirstOrDefault(e => e.DiscordId == socketUser.Id);
        }
        return null;
    }

    public static void LevenshteinOrder(this List<Purchase> purchases)
    {
        if (purchases == null || purchases.Count < 3)
        {
            return;
        }


        for (int i = 0; i < purchases.Count; i++)
        {
            // if changed reset from start
            if (OrderChange(purchases, i))
            {
                i = -1;
            }
        }
    }

    private static bool OrderChange(List<Purchase> data, int index)
    {
        int leastValue = int.MaxValue;
        int leastIndex = -1;

        for (int i = index; i < data.Count; i++)
        {
            if (i == index) continue;
            int value = Fastenshtein.Levenshtein.Distance(data[index].Product, data[i].Product);
            if (value < leastValue)
            {
                leastIndex = i;
                leastValue = value;
            }
        }

        if (leastIndex > index + 1)
        {
            int before = int.MaxValue;
            int after = int.MaxValue;
            if (index > 0)
            {
                before = Fastenshtein.Levenshtein.Distance(data[index - 1].Product, data[index].Product);
            }
            if (leastIndex - index > 1)
            {
                after = Fastenshtein.Levenshtein.Distance(data[index + 1].Product, data[index].Product);
            }

            if (before < after)
            {
                Switch(data, index, leastIndex);
            }
            else
            {
                Switch(data, index + 1, leastIndex);
            }

            return true;
        }
        else
        {
            return false;
        }
    }
    private static void Switch(List<Purchase> data, int to, int from)
    {
        data.Insert(to, data[from]);
        data.RemoveAt(from + 1);
    }


}
