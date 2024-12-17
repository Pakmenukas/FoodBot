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
    public sealed class OrdersRemoveDeliveryCommand(ulong initiatorUserId, int amount) : IRequest<Result<OrdersRemoveDeliveryCommand.Response>>
    {
        public sealed record Response(List<Purchase> PurchaseList, int amountDividedByUsers);
        private ulong InitiatorUserId => initiatorUserId;
        private int Amount => amount;


        public sealed class Handler(MainContext context, ILogger logger) : IRequestHandler<OrdersRemoveDeliveryCommand, Result<Response>>
        {
            public async Task<Result<Response>> Handle(OrdersRemoveDeliveryCommand request, CancellationToken cancellationToken)
            {
                var initiatorUser = await context.Users.FirstOrDefaultAsync(e => e.DiscordId == request.InitiatorUserId, cancellationToken);

                if (initiatorUser is null)
                {
                    var error = new NotFoundError(NotFoundError.ErrorCode.Initiator);
                    await logger.LogError(request.InitiatorUserId, nameof(OrdersRemoveDeliveryCommand), error);
                    return error;
                }

                if (initiatorUser.Role != Role.Root)
                {
                    var error = new ForbiddenError(ForbiddenError.ErrorCode.RootRequired);
                    await logger.LogError(request.InitiatorUserId, nameof(OrdersRemoveDeliveryCommand), error);
                    return error;
                }

                if (request.Amount > 10_00)
                {
                    var error = new ForbiddenError(ForbiddenError.ErrorCode.TransactionLimitReached);
                    await logger.LogError(request.InitiatorUserId, nameof(OrdersRemoveDeliveryCommand), error);
                    return error;
                }

                var order = context.Orders
                    .Include(e => e.PurchaseList)
                        .ThenInclude(f => f.User)
                    .OrderByDescending(e => e.DateCreated)
                    .First() ?? throw new Exception("Empty orders table");

                if (order.IsComplete == false)
                {
                    var error = new ForbiddenError(ForbiddenError.ErrorCode.IncompleteOrder);
                    await logger.LogError(request.InitiatorUserId, nameof(OrdersRemoveDeliveryCommand), error);
                    return error;
                }

                int count = order.PurchaseList.Count;
                int amountDividedByUsers = (int)Math.Round(Math.Ceiling((float)request.Amount / count));


                foreach (var purchase in order.PurchaseList)
                {
                    purchase.User.Money -= amountDividedByUsers;
                }

                context.SaveChanges();

                await logger.LogSuccess(request.InitiatorUserId, nameof(OrdersRemoveDeliveryCommand));

                return new Response(order.PurchaseList, amountDividedByUsers);
            }
        }
    }
}
