using FoodBot.Application.Common;
using FoodBot.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Kitchen
{
    public sealed class IdrinkLeaderboardQuery(ulong initiatorUserId) : IRequest<Result<IdrinkLeaderboardQuery.Response>>
    {
        public sealed record Response(List<IdrinkLeaderboardItem> IdrinkLeaderboardList);
        public sealed record IdrinkLeaderboardItem(string UserName, ulong UserId, int DrinkCount);
        private ulong InitiatorUserId => initiatorUserId;


        public sealed class Handler(MainContext context, ILogger logger) : IRequestHandler<IdrinkLeaderboardQuery, Result<Response>>
        {
            public async Task<Result<Response>> Handle(IdrinkLeaderboardQuery request, CancellationToken cancellationToken)
            {
                var leaderboard = context.Logs
                    .Include(x => x.User)
                    .Where(x => x.Command == "idrink" || x.Command == "PurchaseDrinkCommand")
                    .GroupBy(x => x.User)
                    .ToList()
                    .Select(g => new IdrinkLeaderboardItem(
                        g.Key.Name,
                        g.Key.DiscordId,
                        g.Count()
                    ))
                    .OrderByDescending(x => x.DrinkCount)
                    .ToList();

                await logger.LogSuccess(request.InitiatorUserId, nameof(IdrinkLeaderboardQuery));

                return new Response(leaderboard);
            }
        }
    }
}
