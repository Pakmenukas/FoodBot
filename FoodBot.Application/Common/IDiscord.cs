using FoodBot.Domain;
using MyResult;

namespace FoodBot.Application.Common;

public interface IDiscord
{
    public Task<Result<User>> GetUser(ulong userId, CancellationToken cancellationToken = default);
}