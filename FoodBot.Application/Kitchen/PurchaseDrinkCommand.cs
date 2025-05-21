using FoodBot.Application.Common;
using FoodBot.Application.Errors;
using FoodBot.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Kitchen;

public sealed class PurchaseDrinkCommand(ulong initiatorUserId) : IRequest<Result<PurchaseDrinkCommand.Response>>
{
    public sealed record Response(
        int Amount, 
        int Count,
        int InitiatorOldAmount,
        int InitiatorNewAmount,
        ulong OwnerDiscordId,
        int CommandOwnerOldAmount,
        int CommandOwnerNewAmount);

    private ulong InitiatorUserId => initiatorUserId;

    public sealed class Handler(MainContext context, ILogger logger) : IRequestHandler<PurchaseDrinkCommand, Result<Response>>
    {
        public async Task<Result<Response>> Handle(PurchaseDrinkCommand request, CancellationToken cancellationToken)
        {
            var initiatorUser = await context.Users
                .FirstOrDefaultAsync(e => e.DiscordId == request.InitiatorUserId, cancellationToken);

            if (initiatorUser is null)
            {
                var error = new NotFoundError(NotFoundError.ErrorCode.Initiator);
                await logger.LogError(request.InitiatorUserId, nameof(PurchaseDrinkCommand), error);
                return error;
            }

            if (initiatorUser.Money < -200_00)
            {
                var error = new ForbiddenError(ForbiddenError.ErrorCode.NoFunds);
                await logger.LogError(request.InitiatorUserId, nameof(PurchaseDrinkCommand), error);
                return error;
            }

            var command = await context.Commands
                .Include(e => e.ToUser)
                .FirstAsync(e => e.Name == "idrink", cancellationToken);

            var initiatorOldMoney = initiatorUser.Money;
            var commandOwnerOldMoney = command.ToUser.Money;
            
            var transferAmount = command.Money;
            initiatorUser.Money -= transferAmount;
            command.ToUser.Money += transferAmount;
            command.Count--;
            await context.SaveChangesAsync(cancellationToken);

            return new Response(
                command.Money, 
                command.Count,
                initiatorOldMoney, 
                initiatorUser.Money, 
                command.ToUser.DiscordId,
                commandOwnerOldMoney,
                command.ToUser.Money);
        }
    }
}