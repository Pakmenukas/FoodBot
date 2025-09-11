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
            var discordId = await discord.Authorize(request.Code, cancellationToken);
            if (discordId.IsFailure)
                return discordId.Error;

            var existingUser = context.Users.FirstOrDefault(e => e.DiscordId == discordId.Value);
            if (existingUser is not null) return Result.Ok(existingUser.Id);

            existingUser = new User
            {
                Id = Guid.NewGuid()
            };
            context.Users.Add(existingUser);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Ok(existingUser.Id);
        }
    }
}