using FoodBot.Application.Common;
using FoodBot.Application.Errors;
using FoodBot.Domain;
using FoodBot.Domain.Enums;
using FoodBot.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Bank;

public sealed class AddMoneyCommand(ulong initiatorUserId, AddMoneyCommand.Target target, int amount)
    : IRequest<Result<AddMoneyCommand.Response>>
{
    public sealed record Target(ulong Id, string Name);

    public sealed record Response(int AddedAmount, int NewAmount);

    private ulong InitiatorUserId => initiatorUserId;
    private Target TargetUser => target;
    private int Amount => amount;

    public sealed class Handler(MainContext context, ILogger logger) : IRequestHandler<AddMoneyCommand, Result<Response>>
    {
        public async Task<Result<Response>> Handle(AddMoneyCommand request, CancellationToken cancellationToken)
        {
            var initiatorUser =
                await context.Users.FirstOrDefaultAsync(e => e.DiscordId == request.InitiatorUserId, cancellationToken);
            
            if (initiatorUser is null)
            {
                var error = new NotFoundError(NotFoundError.ErrorCode.Initiator);
                await logger.LogError(request.InitiatorUserId, nameof(AddMoneyCommand), error);
                return error;
            }
            
            if (initiatorUser.Role != Role.Root)
            {
                var error = new ForbiddenError(ForbiddenError.ErrorCode.RootRequired);
                await logger.LogError(request.InitiatorUserId, nameof(AddMoneyCommand), error);
                return error;
            }

            var targetUser =
                await context.Users.FirstOrDefaultAsync(e => e.DiscordId == request.TargetUser.Id, cancellationToken);

            if (targetUser is null)
            {
                targetUser = new User
                {
                    DiscordId = request.TargetUser.Id,
                    Name = request.TargetUser.Name,
                    Role = Role.None,
                    Money = 0,
                };

                context.Users.Add(targetUser);
            }

            targetUser.Money += request.Amount;

            await context.SaveChangesAsync(cancellationToken);

            await logger.LogSuccess(request.InitiatorUserId, nameof(AddMoneyCommand), new List<string> { request.Amount.ToString(), targetUser.Name.ToString(), targetUser.Id.ToString() });
            return new Response(request.Amount, targetUser.Money);
        }
    }
}