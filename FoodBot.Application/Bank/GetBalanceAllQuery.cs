using FoodBot.Application.Common;
using FoodBot.Application.Errors;
using FoodBot.Domain.Enums;
using FoodBot.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Bank;

public sealed class GetBalanceAllQuery(ulong initiatorUserId) : IRequest<Result<List<GetBalanceAllQuery.Response>>>
{
    public sealed record Response(ulong DiscordId, int Amount);
    private ulong InitiatorUserId => initiatorUserId;

    public sealed class Handler(MainContext context, ILogger logger)
        : IRequestHandler<GetBalanceAllQuery, Result<List<Response>>>
    {
        public async Task<Result<List<Response>>> Handle(GetBalanceAllQuery request, CancellationToken cancellationToken)
        {
            var initiatorUser = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.DiscordId == request.InitiatorUserId, cancellationToken);

            if (initiatorUser is null)
            {
                var error = new NotFoundError(NotFoundError.ErrorCode.Initiator);
                await logger.LogError(request.InitiatorUserId, nameof(GetBalanceAllQuery), error);
                return error;
            }

            if (initiatorUser.Role != Role.Root)
            {
                var error = new ForbiddenError(ForbiddenError.ErrorCode.RootRequired);
                await logger.LogError(request.InitiatorUserId, nameof(GetBalanceAllQuery), error);
                return error;
            }
            
            var users = await context.Users
                .AsNoTracking()
                .OrderByDescending(e => e.Money)
                .ToListAsync(cancellationToken);

            await logger.LogSuccess(request.InitiatorUserId, nameof(GetBalanceAllQuery));

            return users.Select(e => new Response(e.DiscordId, e.Money)).ToList();
        }
    }
}