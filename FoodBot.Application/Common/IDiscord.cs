using FoodBot.Domain;
using MyResult;

namespace FoodBot.Application.Common;

public interface IDiscord
{
    public Task<Result<DiscordUser>> Authorize(string code, CancellationToken cancellationToken = default);
    public Task<Result<DiscordUser>> GetUser(ulong userId, CancellationToken cancellationToken = default);
    public Task<Result<List<DiscordUser>>> GetMembers(CancellationToken cancellationToken = default);
}