using FoodBot.Application.Common;
using FoodBot.Application.Errors;
using FoodBot.Domain;
using FoodBot.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Bank;

public sealed class DeleteMeCommand : IRequest<Result>
{
    public sealed class Handler(IMainContext context, ILogger logger, IUserProvider userProvider) : IRequestHandler<DeleteMeCommand, Result>
    {
        public async Task<Result> Handle(DeleteMeCommand request, CancellationToken cancellationToken)
        {
            var initiatorUser =
                await context.Users.FirstOrDefaultAsync(e => e.Id == userProvider.UserId, cancellationToken);
            if (initiatorUser is null)
                return new NotFoundError(NotFoundError.ErrorCode.Initiator);
            
            context.Users.Remove(initiatorUser);
            await context.SaveChangesAsync(cancellationToken);
            await logger.LogSuccess(initiatorUser.Id, nameof(DeleteUserCommand),
                [initiatorUser.Id.ToString(), initiatorUser.DiscordId.ToString(), initiatorUser.Name, "Deleted"]);
            return Result.Ok();
        }
    }
}