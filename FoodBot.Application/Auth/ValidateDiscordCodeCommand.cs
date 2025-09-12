using FoodBot.Application.Common;
using FoodBot.Domain.Enums;
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
            var discordUser = await discord.Authorize(request.Code, cancellationToken);
            if (discordUser.IsFailure)
                return discordUser.Error;
            
            var discordMembers = await discord.GetMembers(cancellationToken);
            if (discordMembers.IsFailure)
                return discordMembers.Error;

            var authedUser = discordMembers.Value.FirstOrDefault(e => e.Id == discordUser.Value.Id);
            if (authedUser is null)
                return new  Error("Unauthorized", "User is not part of the guild");

            var existingUser = context.Users.FirstOrDefault(e => e.DiscordId == discordUser.Value.Id);
            if (existingUser is null)
            {
                existingUser = new Domain.User
                {
                    Id = Guid.NewGuid(),
                    DiscordId = authedUser.Id
                };
                context.Users.Add(existingUser);
            }

            existingUser.Name = authedUser.Name;
            existingUser.AvatarUrl = authedUser.AvatarUrl;
            
            await context.SaveChangesAsync(cancellationToken);

            return existingUser.Id;
        }
    }
}