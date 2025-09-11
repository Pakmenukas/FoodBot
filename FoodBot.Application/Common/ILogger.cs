using MyResult;

namespace FoodBot.Application.Common;

public interface ILogger
{
    public Task LogSuccess(ulong userId, string commandName, List<string>? data = null);
    public Task LogSuccess(Guid userId, string commandName, List<string>? data = null);
    public Task LogError(ulong userId, string commandName, List<string>? data = null);
    public Task LogError(Guid userId, string commandName, List<string>? data = null);
    public Task LogError(ulong userId, string commandName, Error error);
    public Task LogError(Guid userId, string commandName, Error error);
}