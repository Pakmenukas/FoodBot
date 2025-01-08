using FoodBot.Application.Common;
using FoodBot.Application.Errors;
using FoodBot.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Kitchen
{
    public sealed class OrderCancelCommand(ulong initiatorUserId) : IRequest<Result>
    {
        private ulong InitiatorUserId => initiatorUserId;


        public sealed class Handler(MainContext context, ILogger logger) : IRequestHandler<OrderCancelCommand, Result>
        {
            public async Task<Result> Handle(OrderCancelCommand request, CancellationToken cancellationToken)
            {
                var initiatorUser = await context.Users.FirstOrDefaultAsync(e => e.DiscordId == request.InitiatorUserId, cancellationToken);

                if (initiatorUser is null)
                {
                    var error = new NotFoundError(NotFoundError.ErrorCode.Initiator);
                    await logger.LogError(request.InitiatorUserId, nameof(OrderCancelCommand), error);
                    return error;
                }

                var lastIncomplete = context.Orders
                    .Include(e => e.PurchaseList)
                    .Where(e => !e.IsComplete)
                    .OrderByDescending(e => e.DateCreated)
                    .FirstOrDefault();

                if (lastIncomplete is null || !lastIncomplete.PurchaseList.Any(e => e.User == initiatorUser))
                {
                    var error = new NotFoundError(NotFoundError.ErrorCode.Order);
                    await logger.LogError(request.InitiatorUserId, nameof(OrderCancelCommand), error);
                    return error;
                }

                context.Purchases.Remove(lastIncomplete.PurchaseList.First(e => e.User == initiatorUser));
                context.SaveChanges();

                await logger.LogSuccess(request.InitiatorUserId, nameof(OrderCancelCommand));
                return Result.Ok();
            }
        }
    }
}
