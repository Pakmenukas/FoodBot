using FoodBot.Application.Common;
using FoodBot.Application.Errors;
using FoodBot.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Bank;

public sealed class TransferMoneyCommand(ulong initiatorUserId, ulong targetUserId, int amount)
    : IRequest<Result<TransferMoneyCommand.Response>>
{
    public sealed record Response(int Amount,
        int InitiatorOldAmount,
        int InitiatorNewAmount,
        int TargetOldAmount,
        int TargetNewAmount);

    private ulong InitiatorUserId => initiatorUserId;
    private ulong TargetUserId => targetUserId;
    private int Amount => amount;

    public sealed class Handler(MainContext context, ILogger logger)
        : IRequestHandler<TransferMoneyCommand, Result<Response>>
    {
        public async Task<Result<Response>> Handle(TransferMoneyCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
            {
                var error = new BadRequestError(BadRequestError.ErrorCode.AmountTooLow);
                await logger.LogError(request.InitiatorUserId, nameof(TransferMoneyCommand), error);
                return error;
            }
            
            if (request.Amount > 100_00)
            {
                var error = new ForbiddenError(ForbiddenError.ErrorCode.TransactionLimitReached);
                await logger.LogError(request.InitiatorUserId, nameof(TransferMoneyCommand), error);
                return error;
            }
            
            if (request.InitiatorUserId == request.TargetUserId)
            {
                var error = new ForbiddenError(ForbiddenError.ErrorCode.IllegalAction);
                await logger.LogError(request.InitiatorUserId, nameof(TransferMoneyCommand), error);
                return error;
            }

            var initiatorUser =
                await context.Users.FirstOrDefaultAsync(e => e.DiscordId == request.InitiatorUserId, cancellationToken);

            if (initiatorUser is null)
            {
                var error = new NotFoundError(NotFoundError.ErrorCode.Initiator);
                await logger.LogError(request.InitiatorUserId, nameof(TransferMoneyCommand), error);
                return error;
            }
            
            if (initiatorUser.Money < -200_00)
            {
                var error = new ForbiddenError(ForbiddenError.ErrorCode.NoFunds);
                await logger.LogError(request.InitiatorUserId, nameof(TransferMoneyCommand), error);
                return error;
            }

            var targetUser =
                await context.Users.FirstOrDefaultAsync(e => e.DiscordId == request.TargetUserId, cancellationToken);

            if (targetUser is null)
            {
                var error = new NotFoundError(NotFoundError.ErrorCode.TargetUser);
                await logger.LogError(request.InitiatorUserId, nameof(TransferMoneyCommand), error);
                return error;
            }

            var initiatorOldMoneyAmount = initiatorUser.Money;
            var targetOldMoneyAmount = targetUser.Money;
            
            initiatorUser.Money -= request.Amount;
            targetUser.Money += request.Amount;

            await context.SaveChangesAsync(cancellationToken);

            await logger.LogSuccess(request.InitiatorUserId, nameof(TransferMoneyCommand));
            return new Response(request.Amount, initiatorOldMoneyAmount, initiatorUser.Money, targetOldMoneyAmount, targetUser.Money);
        }
    }
}