using FoodBot.Application.Common;
using FoodBot.Domain;
using MediatR;
using MyResult;

namespace FoodBot.Application;

public class TestQuery : IRequest<Result<User>>
{
    public sealed class Handler(IDiscord discord) : IRequestHandler<TestQuery, Result<User>>
    {
        public Task<Result<User>> Handle(TestQuery request, CancellationToken cancellationToken)
        {
            return discord.GetUser(123, cancellationToken);
        }
    }
}