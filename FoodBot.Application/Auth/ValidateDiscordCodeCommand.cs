using FoodBot.Application.Common;
using FoodBot.Domain;
using MediatR;
using MyResult;

namespace FoodBot.Application.Auth;

public sealed class ValidateDiscordCodeCommand(string code) : IRequest<Result<Guid>>
{
    private string Code => code;

    public sealed class Handler(IMainContext context, IDiscord discord) : IRequestHandler<ValidateDiscordCodeCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ValidateDiscordCodeCommand request, CancellationToken cancellationToken)
        {
            var user = await discord.Authorize(request.Code, cancellationToken);
            if (user.IsFailure)
                return user.Error;

            var existingUser = context.Users.FirstOrDefault(e => e.DiscordId == user.Value.DiscordId);
            if (existingUser is null)
            {
                existingUser = new Domain.User
                {
                    Id = Guid.NewGuid()
                };
                context.Users.Add(existingUser);
            }

            existingUser.Name = user.Value.Name;
            existingUser.AvatarUrl = user.Value.AvatarUrl;
            
            await context.SaveChangesAsync(cancellationToken);

            return Result.Ok(existingUser.Id);
        }
    }
}