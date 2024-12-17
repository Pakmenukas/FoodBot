using FoodBot.Application.Common;
using FoodBot.Application.Errors;
using FoodBot.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Kitchen
{
    public sealed class OrderMoneyCommand(ulong initiatorUserId, int amount) : IRequest<Result<OrderMoneyCommand.Response>>
    {
        public sealed record Response(string Product, int Amount);
        private ulong InitiatorUserId => initiatorUserId;
        private int Amount => amount;


        public sealed class Handler(MainContext context, ILogger logger) : IRequestHandler<OrderMoneyCommand, Result<Response>>
        {
            public async Task<Result<Response>> Handle(OrderMoneyCommand request, CancellationToken cancellationToken)
            {
                var initiatorUser = await context.Users.FirstOrDefaultAsync(e => e.DiscordId == request.InitiatorUserId, cancellationToken);

                if (initiatorUser is null)
                {
                    var error = new NotFoundError(NotFoundError.ErrorCode.Initiator);
                    await logger.LogError(request.InitiatorUserId, nameof(OrderMoneyCommand), error);
                    return error;
                }

                var lastIncomplete = context.Orders
                    .Include(e => e.PurchaseList)
                    .Where(e => !e.IsComplete)
                    .OrderByDescending(e => e.DateCreated)
                    .FirstOrDefault();

                if (request.Amount <= 0)
                {
                    var error = new BadRequestError(BadRequestError.ErrorCode.AmountTooLow);
                    await logger.LogError(request.InitiatorUserId, nameof(OrderMoneyCommand), error);
                    return error;
                }

                if (request.Amount > 100_00)
                {
                    var error = new ForbiddenError(ForbiddenError.ErrorCode.TransactionLimitReached);
                    await logger.LogError(request.InitiatorUserId, nameof(OrderMoneyCommand), error);
                    return error;
                }

                if (lastIncomplete is null || !lastIncomplete.PurchaseList.Any(e => e.User == initiatorUser))
                {
                    var error = new NotFoundError(NotFoundError.ErrorCode.Order);
                    await logger.LogError(request.InitiatorUserId, nameof(OrderMoneyCommand), error);
                    return error;
                }

                var purchase = lastIncomplete.PurchaseList.First(e => e.User == initiatorUser);
                purchase.Money = request.Amount;

                context.SaveChanges();

                await logger.LogSuccess(request.InitiatorUserId, nameof(OrderMoneyCommand));

                return new Response(purchase.Product, purchase.Money);
            }
        }
    }
}
