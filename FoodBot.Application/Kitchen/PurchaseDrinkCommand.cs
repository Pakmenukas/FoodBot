using FoodBot.Application.Common;
using FoodBot.Application.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Kitchen;

public sealed class PurchaseDrinkCommand : IRequest<Result<PurchaseDrinkCommand.Response>>
{
    public record Response(int TotalPurchasedCount, int WeeklyPurchased, int MonthlyPurchased, int YearlyPurchased);
    public sealed class Handler(IMainContext context, ISender mediator, IUserProvider userProvider) 
        : IRequestHandler<PurchaseDrinkCommand, Result<Response>>
    {
        public async Task<Result<Response>> Handle(PurchaseDrinkCommand request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == userProvider.UserId, cancellationToken);
            if (user is null)
                return new NotFoundError(NotFoundError.ErrorCode.Initiator);

            await mediator.Send(new Discord.Kitchen.PurchaseDrinkCommand(user.DiscordId), cancellationToken);
            
            var purchasedDrinks = await mediator.Send(new DrinkCountQuery(), cancellationToken);
            if (purchasedDrinks.IsFailure)
                return purchasedDrinks.Error;

            return new Response(purchasedDrinks.Value.TotalPurchasedCount, purchasedDrinks.Value.WeeklyPurchased,
                purchasedDrinks.Value.MonthlyPurchased, purchasedDrinks.Value.YearlyPurchased);
        }
    }
}