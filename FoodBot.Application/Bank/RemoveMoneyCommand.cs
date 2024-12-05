using FoodBot.Application.Common;
using FoodBot.Application.Errors;
using FoodBot.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Bank;

public sealed class RemoveMoneyCommand(ulong initiatorUserId, int amount)
    : IRequest<Result<RemoveMoneyCommand.Response>>
{
    public sealed record Response(int RemovedAmount, int NewAmount);

    private ulong InitiatorUserId => initiatorUserId;
    private int Amount => amount;

    public sealed class Handler(MainContext context, ILogger logger)
        : IRequestHandler<RemoveMoneyCommand, Result<Response>>
    {
        public async Task<Result<Response>> Handle(RemoveMoneyCommand request, CancellationToken cancellationToken)
        {
            var initiatorUser =
                await context.Users.FirstOrDefaultAsync(e => e.DiscordId == request.InitiatorUserId, cancellationToken);

            if (initiatorUser is null)
            {
                var error = new NotFoundError(NotFoundError.ErrorCode.Initiator);
                await logger.LogError(request.InitiatorUserId, nameof(RemoveMoneyCommand), error);
                return error;
            }

            initiatorUser.Money -= request.Amount;

            await context.SaveChangesAsync(cancellationToken);

            await logger.LogSuccess(request.InitiatorUserId, nameof(RemoveMoneyCommand));
            return new Response(request.Amount, initiatorUser.Money);
        }
    }
}