using FoodBot.Application.Common;
using FoodBot.Application.Errors;
using FoodBot.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Bank;

public sealed class GetBalanceQuery(ulong initiatorUserId) : IRequest<Result<int>>
{
    private ulong InitiatorUserId => initiatorUserId;

    public sealed class Handler(MainContext context, ILogger logger)
        : IRequestHandler<GetBalanceQuery, Result<int>>
    {
        public async Task<Result<int>> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
        {
            var initiatorUser = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.DiscordId == request.InitiatorUserId, cancellationToken);

            if (initiatorUser is null)
            {
                var error = new NotFoundError(NotFoundError.ErrorCode.Initiator);
                await logger.LogError(request.InitiatorUserId, nameof(GetBalanceQuery), error);
                return error;
            }

            await logger.LogSuccess(request.InitiatorUserId, nameof(GetBalanceQuery));

            return initiatorUser.Money;
        }
    }
}