using FoodBot.Domain;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Common;

public sealed class LoggerService(IMainContext context) : ILogger
{
    public async Task LogSuccess(ulong userId, string commandName, List<string>? data = null)
    {
        var user = await GetUser(userId);
        await Log(user, true, commandName, data);
    }

    public async Task LogSuccess(Guid userId, string commandName, List<string>? data = null)
    {
        var user = await GetUser(userId);
        await Log(user, true, commandName, data);
    }

    public async Task LogError(ulong userId, string commandName, List<string>? data = null)
    {
        var user = await GetUser(userId);
        await Log(user, false, commandName, data);
    }

    public async Task LogError(Guid userId, string commandName, List<string>? data = null)
    {
        var user = await GetUser(userId);
        await Log(user, false, commandName, data);
    }

    public async Task LogError(ulong userId, string commandName, Error error)
    {
        var user = await GetUser(userId);
        await Log(user, false, commandName, [error.ToString()]);
    }

    public async Task LogError(Guid userId, string commandName, Error error)
    {
        var user = await GetUser(userId);
        await Log(user, false, commandName, [error.ToString()]);
    }

    private async Task<Domain.User?> GetUser(ulong userId)
    {
        return await context.Users.AsNoTracking().FirstOrDefaultAsync(e => e.DiscordId == userId);
    }

    private async Task<Domain.User?> GetUser(Guid userId)
    {
        return await context.Users.AsNoTracking().FirstOrDefaultAsync(e => e.Id == userId);
    }

    private async Task Log(Domain.User? user, bool isSuccess, string commandName, List<string>? data = null)
    {
        Console.WriteLine($"{user?.DiscordId}: {commandName}\n{string.Join('|', data ?? [])}");
        try
        {
            var log = new Log
            {
                Date = DateTime.Now,
                UserId = user?.Id,
                Success = isSuccess,
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
}