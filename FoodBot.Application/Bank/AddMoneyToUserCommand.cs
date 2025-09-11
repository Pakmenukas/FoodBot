using FoodBot.Application.Common;
using FoodBot.Application.Errors;
using FoodBot.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Bank;

public sealed class AddMoneyToUserCommand : IRequest<Result>
{
    public required Guid UserId { get; init; }
    public required decimal Amount { get; init; }

    public sealed class Handler(IMainContext context, ISender mediator, IUserProvider userProvider) : IRequestHandler<AddMoneyToUserCommand, Result>
    {
        public async Task<Result> Handle(AddMoneyToUserCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount.Scale > 2)
                return new BadRequestError(BadRequestError.ErrorCode.InvalidAmountNumberFormat);

            var initiatorUser = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == userProvider.UserId);
            if (initiatorUser is null)
                return new NotFoundError(NotFoundError.ErrorCode.Initiator);
            if (initiatorUser.Role != Role.Root)
                return new ForbiddenError(ForbiddenError.ErrorCode.RootRequired);

            var targetUser = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.UserId);
            if (targetUser is null)
                return new NotFoundError(NotFoundError.ErrorCode.TargetUser);

            var result = await mediator.Send(new AddMoneyCommand(
                targetUser.DiscordId,
                new AddMoneyCommand.Target(targetUser.DiscordId, targetUser.Name),
                Convert.ToInt32(request.Amount * 100)), cancellationToken);
            if (result.IsFailure)
                return result.Error;;
            
            return Result.Ok();
        }
    }
}