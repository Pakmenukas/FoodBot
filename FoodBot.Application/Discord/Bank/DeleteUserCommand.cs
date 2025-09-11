using FoodBot.Application.Common;
using FoodBot.Application.Errors;
using FoodBot.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.Discord.Bank;

public sealed class DeleteUserCommand(Guid userId) : IRequest<Result>
{
    private Guid UserId => userId;

    public sealed class Handler(IMainContext context, ILogger logger, IUserProvider userProvider) : IRequestHandler<DeleteUserCommand, Result>
    {
        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var initiatorUser =
                await context.Users.FirstOrDefaultAsync(e => e.Id == userProvider.UserId, cancellationToken);
            if (initiatorUser is null)
                return new NotFoundError(NotFoundError.ErrorCode.Initiator);
            if (initiatorUser.Role != Role.Root)
                return new ForbiddenError(ForbiddenError.ErrorCode.RootRequired);
            
            var user = await context.Users.FirstOrDefaultAsync(e => e.Id == request.UserId, cancellationToken);
            if (user is null)
                return new NotFoundError(NotFoundError.ErrorCode.TargetUser);
            
            context.Users.Remove(user);
            await context.SaveChangesAsync(cancellationToken);
            await logger.LogSuccess(request.UserId, nameof(DeleteUserCommand),
                [user.Id.ToString(), user.DiscordId.ToString(), user.Name, "Deleted"]);
            return Result.Ok();
        }
    }
}