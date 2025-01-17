using FoodBot.Application.Bank;
using FoodBot.Application.Common;
using FoodBot.Application.Errors;
using FoodBot.Domain;
using FoodBot.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Kitchen
{
    public sealed class OrderCommand(ulong initiatorUserId, string product, int amount) : IRequest<Result>
    {
        private ulong InitiatorUserId => initiatorUserId;
        private string Product => product;
        private int Amount => amount;

        public sealed class Handler(MainContext context, ILogger logger) : IRequestHandler<OrderCommand, Result>
        {
            public async Task<Result> Handle(OrderCommand request, CancellationToken cancellationToken)
            {
                var initiatorUser =
                    await context.Users.FirstOrDefaultAsync(e => e.DiscordId == request.InitiatorUserId, cancellationToken);

                if (initiatorUser is null)
                {
                    var error = new NotFoundError(NotFoundError.ErrorCode.Initiator);
                    await logger.LogError(request.InitiatorUserId, nameof(OrderCommand), error);
                    return error;
                }

                if (request.Amount < 0)
                {
                    var error = new BadRequestError(BadRequestError.ErrorCode.AmountTooLow);
                    await logger.LogError(request.InitiatorUserId, nameof(OrderCommand), error);
                    return error;
                }

                if (request.Amount > 100_00)
                {
                    var error = new ForbiddenError(ForbiddenError.ErrorCode.TransactionLimitReached);
                    await logger.LogError(request.InitiatorUserId, nameof(OrderCommand), error);
                    return error;
                }

                AddOrder(initiatorUser, request.Product, request.Amount);

                await logger.LogSuccess(request.InitiatorUserId, nameof(OrderCommand));
                return Result.Ok();
            }

            private void AddOrder(User initiator, string product, int money)
            {
                var lastIncomplete = context.Orders
                    .Include(e => e.PurchaseList)
                    .Where(e => !e.IsComplete)
                    .OrderByDescending(e => e.DateCreated)
                    .FirstOrDefault();

                if (lastIncomplete is null)
                {
                    lastIncomplete = new Order
                    {
                        DateCreated = DateTime.Now,
                        IsComplete = false,
                    };
                    context.Orders.Add(lastIncomplete);
                    context.SaveChanges();
                }
                var existing = lastIncomplete.PurchaseList.FirstOrDefault(e => e.User == initiator);

                if (existing is null)
                {
                    existing = new Purchase();
                    existing.User = initiator;
                    existing.Order = lastIncomplete;
                    context.Purchases.Add(existing);
                }
                existing.Money = money;
                existing.Product = product;
                existing.Date = DateTime.Now;

                try
                {
                    context.SaveChanges();
                }
                catch (Exception)
                {
                    throw;
                }
            }

        }
    }
}
