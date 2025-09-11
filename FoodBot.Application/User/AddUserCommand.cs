using FoodBot.Application.Common;
using FoodBot.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.User;

public sealed class AddUserCommand : IRequest<Result>
{
    public required string DiscordId { get; init; }
    
    public sealed class Handler(IMainContext context, IDiscord discord) : IRequestHandler<AddUserCommand, Result>
    {
        public async Task<Result> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            var idParseSuccess = ulong.TryParse(request.DiscordId, out var discordIdSnowflake);
            if (!idParseSuccess) return new Error("Invalid Discord Id", "Discord Id is invalid");
            var discordUser = await discord.GetUser(discordIdSnowflake, cancellationToken);
            if (discordUser.IsFailure) return new Error("Invalid Discord Id", "Discord Id is invalid");

            var newUser = new Domain.User
            {
                Id = Guid.NewGuid(),
                DiscordId = discordUser.Value.Id,
                Name = discordUser.Value.Name,
                AvatarUrl = discordUser.Value.AvatarUrl,
                Role = Role.None,
                Money = 0,
                NoGarbage = false,
            };
            context.Users.Add(newUser);
            await context.SaveChangesAsync(cancellationToken);
            
            return Result.Ok();
        }
    }
}