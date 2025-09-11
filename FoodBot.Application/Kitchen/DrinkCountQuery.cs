using FoodBot.Application.Common;
using FoodBot.Application.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Kitchen;

public sealed class DrinkCountQuery : IRequest<Result<DrinkCountQuery.Response>>
{
    public record Response(int TotalPurchasedCount, int WeeklyPurchased, int MonthlyPurchased, int YearlyPurchased);
    
    public sealed class Handler(IMainContext context, IUserProvider userProvider) 
        : IRequestHandler<DrinkCountQuery, Result<Response>>
    {
        public async Task<Result<Response>> Handle(DrinkCountQuery request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == userProvider.UserId, cancellationToken);
            if (user is null)
                return new NotFoundError(NotFoundError.ErrorCode.Initiator);
            
            var now = DateTime.UtcNow;

            var query = context.Logs
                .Include(x => x.User)
                .Where(x => x.User!.Id == user.Id)
                .Where(x => x.Command == "idrink" || x.Command == "PurchaseDrinkCommand");

            var totalPurchasedDrinksTask = query
                .CountAsync(cancellationToken);

            var weeklyPurchasedDrinksTask = query
                .CountAsync(x => x.Date >= GetStartOfWeek(now), cancellationToken);

            var monthlyPurchasedDrinksTask = query
                .CountAsync(x => x.Date >= new DateTime(now.Year, now.Month, 1), cancellationToken);

            var yearlyPurchasedDrinksTask = query
                .CountAsync(x => x.Date >= new DateTime(now.Year, 1, 1), cancellationToken);

            await Task.WhenAll(totalPurchasedDrinksTask, weeklyPurchasedDrinksTask, monthlyPurchasedDrinksTask,
                yearlyPurchasedDrinksTask);

            return new Response(totalPurchasedDrinksTask.Result, weeklyPurchasedDrinksTask.Result,
                monthlyPurchasedDrinksTask.Result, yearlyPurchasedDrinksTask.Result);
        }
        private static DateTime GetStartOfWeek(DateTime date)
        {
            var daysSinceMonday = ((int)date.DayOfWeek + 6) % 7;
            return date.Date.AddDays(-daysSinceMonday);
        }
    }
}