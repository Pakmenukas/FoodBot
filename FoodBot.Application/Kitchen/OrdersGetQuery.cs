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
    public sealed class OrdersGetQuery(ulong initiatorUserId) : IRequest<Result<OrdersGetQuery.Response>>
    {
        public sealed record Response(List<Purchase> PurchaseList);
        private ulong InitiatorUserId => initiatorUserId;


        public sealed class Handler(MainContext context, ILogger logger) : IRequestHandler<OrdersGetQuery, Result<Response>>
        {
            public async Task<Result<Response>> Handle(OrdersGetQuery request, CancellationToken cancellationToken)
            {
                var initiatorUser = await context.Users.FirstOrDefaultAsync(e => e.DiscordId == request.InitiatorUserId, cancellationToken);

                if (initiatorUser is null)
                {
                    var error = new NotFoundError(NotFoundError.ErrorCode.Initiator);
                    await logger.LogError(request.InitiatorUserId, nameof(OrdersGetQuery), error);
                    return error;
                }

                if (initiatorUser.Role != Role.Root)
                {
                    var error = new ForbiddenError(ForbiddenError.ErrorCode.RootRequired);
                    await logger.LogError(request.InitiatorUserId, nameof(OrdersGetQuery), error);
                    return error;
                }

                var lastIncomplete = context.Orders
                    .Include(e => e.PurchaseList)
                        .ThenInclude(f => f.User)
                    .Where(e => !e.IsComplete)
                    .OrderByDescending(e => e.DateCreated)
                    .FirstOrDefault();

                if (lastIncomplete is null)
                {
                    var error = new NotFoundError(NotFoundError.ErrorCode.Order);
                    await logger.LogError(request.InitiatorUserId, nameof(OrdersGetQuery), error);
                    return error;
                }

                GetChances(lastIncomplete.PurchaseList);

                await logger.LogSuccess(request.InitiatorUserId, nameof(OrdersGetQuery));

                return new Response(lastIncomplete.PurchaseList);
            }

            private void GetChances(List<Purchase> purchases)
            {
                int baseChance = 0;
                int rangeCheck = 15;

                var orderList = context.Orders.OrderByDescending(e => e.DateCreated).Where(e => e.IsComplete);

                // get base chances
                for (int i = 0; i < purchases.Count; i++)
                {
                    var p = purchases[i];

                    bool garbageLast1 = orderList.Take(1).Where(e => e.GarbagePerson == p.User).Any();
                    int garbageLastRange = orderList.Take(rangeCheck).Where(e => e.GarbagePerson == p.User).Count();

                    int chance = 0;
                    if (!garbageLast1)
                    {
                        chance += baseChance + ((rangeCheck - garbageLastRange) * (rangeCheck - garbageLastRange));
                    }
                    chance += (p.User.Money < 0 ? Math.Abs(p.User.Money) : 0);
                    p.ChanceInt = chance;
                }

                // remove root no garbage persons
                purchases.ForEach(e =>
                {
                    if (e.User.NoGarbage)
                    {
                        e.ChanceInt = 0;
                    }
                });

                // check if not all zero
                if (purchases.Sum(e => e.ChanceInt) == 0)
                {
                    purchases.ForEach(e =>
                    {
                        e.ChanceInt = 1;
                    });
                }
            }
        }
    }
}
