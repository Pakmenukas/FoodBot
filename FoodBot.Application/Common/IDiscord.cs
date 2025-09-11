using MyResult;

namespace FoodBot.Application.Common;

public interface IDiscord
{
    public Task<Result<Domain.User>> Authorize(string code, CancellationToken cancellationToken = default);
    public Task<Result<Domain.User>> GetUser(ulong userId, CancellationToken cancellationToken = default);
}