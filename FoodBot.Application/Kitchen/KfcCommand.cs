using FoodBot.Application.Common;
using FoodBot.Application.Errors;
using FoodBot.Application.Helpers;
using FoodBot.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Kitchen;

public sealed class KfcCommand(ulong initiatorUserId, string taste, string drink, string extraItems) : IRequest<Result>
{
    private ulong InitiatorUserId => initiatorUserId;
    private string Taste => taste;
    private string Drink => drink;
    private string ExtraItems => extraItems;

    public sealed class Handler(IMainContext context, ILogger logger) : IRequestHandler<KfcCommand, Result>
    {
        public async Task<Result> Handle(KfcCommand request, CancellationToken cancellationToken)
        {
            var initiatorUser = await context.Users.FirstOrDefaultAsync(e => e.DiscordId == request.InitiatorUserId, cancellationToken);

            if (initiatorUser is null)
            {
                var error = new NotFoundError(NotFoundError.ErrorCode.Initiator);
                await logger.LogError(request.InitiatorUserId, nameof(PurchaseDrinkCommand), error);
                return error;
            }

            var orderString = KfcHelpers.ConstructKfcOrderString(request.Taste, request.Drink, request.ExtraItems);
            await AddOrder(initiatorUser, orderString, 0);

            await logger.LogSuccess(request.InitiatorUserId, nameof(KfcCommand));
            return Result.Ok();
        }

        private async Task AddOrder(User initiator, string product, int money)
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
                await context.SaveChangesAsync();
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

            await context.SaveChangesAsync();
        }
    }


}

