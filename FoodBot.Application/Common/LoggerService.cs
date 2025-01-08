using FoodBot.Domain;
using FoodBot.Infrastructure;
using MyResult;

namespace FoodBot.Application.Common;

public sealed class LoggerService(MainContext context) : ILogger
{
    public async Task LogSuccess(ulong userId, string commandName, List<string>? data = null)
    {
        Console.WriteLine($"{userId}: {commandName}\n{string.Join('|', data ?? [])}");
        try
        {
            var log = new Log
            {
                Date = DateTime.Now,
                User = context.Users.FirstOrDefault(e => e.DiscordId == userId),
                Success = true,
                Command = commandName,
                Data = data is not null ? string.Join("|", data.Select(x => x.Replace('|', '/'))) : string.Empty,
            };
            context.Logs.Add(log);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    public async Task LogError(ulong userId, string commandName, List<string>? data = null)
    {
        Console.WriteLine($"{userId}: {commandName}\n{string.Join('|', data ?? [])}");
        try
        {
            var log = new Log
            {
                Date = DateTime.Now,
                User = context.Users.FirstOrDefault(e => e.DiscordId == userId),
                Success = false,
                Command = commandName,
                Data = data is not null ? string.Join("|", data.Select(x => x.Replace('|', '/'))) : string.Empty,
            };
            context.Logs.Add(log);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public Task LogError(ulong userId, string commandName, Error error)
    {
        return LogError(userId, commandName, [error.ToString()]);
    }
}