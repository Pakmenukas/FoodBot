using FoodBot.Application.Common;
using FoodBot.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Kitchen
{
    public sealed class IdrinkLeaderboardQuery(ulong initiatorUserId, IdrinkLeaderboardQuery.LeaderboardPeriod leaderboardPeriod) : IRequest<Result<IdrinkLeaderboardQuery.Response>>
    {
        public sealed record Response(List<IdrinkLeaderboardItem> IdrinkLeaderboardList);
        public sealed record IdrinkLeaderboardItem(string UserName, ulong UserId, int DrinkCount);
        private ulong InitiatorUserId => initiatorUserId;
        private LeaderboardPeriod Period => leaderboardPeriod;

        public enum LeaderboardPeriod
        {
            AllTime,
            Weekly,
            Monthly,
            Yearly
        }

        public sealed class Handler(MainContext context, ILogger logger) : IRequestHandler<IdrinkLeaderboardQuery, Result<Response>>
        {
            public async Task<Result<Response>> Handle(IdrinkLeaderboardQuery request, CancellationToken cancellationToken)
            {
                var query = context.Logs
                    .Include(x => x.User)
                    .Where(x => x.Command == "idrink" || x.Command == "PurchaseDrinkCommand");

                var now = DateTime.UtcNow;

                query = request.Period switch
                {
                    LeaderboardPeriod.Weekly => query.Where(x => x.Date >= GetStartOfWeek(now)),
                    LeaderboardPeriod.Monthly => query.Where(x => x.Date >= new DateTime(now.Year, now.Month, 1)),
                    LeaderboardPeriod.Yearly => query.Where(x => x.Date >= new DateTime(now.Year, 1, 1)),
                    LeaderboardPeriod.AllTime => query,
                    _ => query
                };

                var leaderboard = query
                    .GroupBy(x => x.User)
                    .ToList()
                    .Select(g => new IdrinkLeaderboardItem(
                        g.Key.Name,
                        g.Key.DiscordId,
                        g.Count()
                    ))
                    .OrderByDescending(x => x.DrinkCount)
                        .ThenBy(x => x.UserName)
                    .ToList();

                await logger.LogSuccess(request.InitiatorUserId, nameof(IdrinkLeaderboardQuery));
                return new Response(leaderboard);
            }

            private static DateTime GetStartOfWeek(DateTime date)
            {
                var daysSinceMonday = ((int)date.DayOfWeek + 6) % 7;
                return date.Date.AddDays(-daysSinceMonday);
            }
        }
    }
}
