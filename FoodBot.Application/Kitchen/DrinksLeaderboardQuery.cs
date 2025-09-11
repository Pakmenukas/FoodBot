using FoodBot.Application.Common;
using FoodBot.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Kitchen
{
    public sealed class DrinksLeaderboardQuery : IRequest<Result<DrinksLeaderboardQuery.Response>>
    {
        public sealed record Response(
            List<IdrinkLeaderboardItem> Total, 
            List<IdrinkLeaderboardItem> Weekly, 
            List<IdrinkLeaderboardItem> Monthly, 
            List<IdrinkLeaderboardItem> Yearly);

        public sealed record IdrinkLeaderboardItem(User User, int DrinkCount);
        public sealed record User(string Name, string? AvatarUrl);

        public sealed class Handler(IMainContext context) : IRequestHandler<DrinksLeaderboardQuery, Result<Response>>
        {
            public async Task<Result<Response>> Handle(DrinksLeaderboardQuery request, CancellationToken cancellationToken)
            {
                var query = context.Logs
                    .Include(x => x.User)
                    .Where(x => x.Command == "idrink" || x.Command == "PurchaseDrinkCommand");

                var now = DateTime.UtcNow;

                var totalLeaderboard = GetLeaderboardItems(query, now, cancellationToken);
                var weeklyLeaderboard = GetLeaderboardItems(query, GetStartOfWeek(now), cancellationToken);
                var monthlyLeaderboard = GetLeaderboardItems(query, new DateTime(now.Year, now.Month, 1), cancellationToken);
                var yearlyLeaderboard = GetLeaderboardItems(query, new DateTime(now.Year, 1, 1), cancellationToken);
                
                // Optimize later
                // await Task.WhenAll(totalLeaderboardTask, weeklyLeaderboardTask, monthlyLeaderboardTask, yearlyLeaderboardTask);

                return new Response(totalLeaderboard,
                    weeklyLeaderboard,
                    monthlyLeaderboard,
                    yearlyLeaderboard);
            }

            private static List<IdrinkLeaderboardItem> GetLeaderboardItems(IQueryable<Log> query,
                DateTime dateLimit, CancellationToken cancellationToken)
            {
                return query
                    .Where(x => x.Date >= dateLimit)
                    .GroupBy(x => x.User)
                    .ToList()
                    .Select(g => new IdrinkLeaderboardItem(
                        new User(g.Key!.Name, g.Key.AvatarUrl),
                        g.Count()
                    ))
                    .OrderByDescending(x => x.DrinkCount)
                    .ThenBy(x => x.User.Name)
                    .ToList();
            }

            private static DateTime GetStartOfWeek(DateTime date)
            {
                var daysSinceMonday = ((int)date.DayOfWeek + 6) % 7;
                return date.Date.AddDays(-daysSinceMonday);
            }
        }
    }
}
