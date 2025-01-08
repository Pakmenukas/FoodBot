using FoodBot.Application.Common;
using FoodBot.Application.Errors;
using FoodBot.Domain;
using FoodBot.Domain.Enums;
using FoodBot.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Kitchen
{
    public sealed class RandomUserQuery(ulong initiatorUserId) : IRequest<Result<RandomUserQuery.Response>>
    {
        public sealed record Response(User randomUser, bool IsComplete);
        private ulong InitiatorUserId => initiatorUserId;


        public sealed class Handler(MainContext context, ILogger logger) : IRequestHandler<RandomUserQuery, Result<Response>>
        {
            public async Task<Result<Response>> Handle(RandomUserQuery request, CancellationToken cancellationToken)
            {
                var initiatorUser = await context.Users.FirstOrDefaultAsync(e => e.DiscordId == request.InitiatorUserId, cancellationToken);

                if (initiatorUser is null)
                {
                    var error = new NotFoundError(NotFoundError.ErrorCode.Initiator);
                    await logger.LogError(request.InitiatorUserId, nameof(OrdersDoneCommand), error);
                    return error;
                }

                if (initiatorUser.Role != Role.Root)
                {
                    var error = new ForbiddenError(ForbiddenError.ErrorCode.RootRequired);
                    await logger.LogError(request.InitiatorUserId, nameof(OrdersDoneCommand), error);
                    return error;
                }

                var order = context.Orders
                    .Include(e => e.PurchaseList).ThenInclude(f => f.User)
                    .OrderByDescending(e => e.DateCreated)
                    .First() ?? throw new Exception("Empty orders table");

                int randomNumber = Utils.GetRandomNumber(order.PurchaseList.Count);

                var randomUser = order.PurchaseList[randomNumber].User;

                await logger.LogSuccess(request.InitiatorUserId, nameof(RandomUserQuery));

                return new Response(randomUser, order.IsComplete);
            }


        }
    }
}
