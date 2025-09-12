using FoodBot.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.User;

public sealed class GetMeQuery : IRequest<Result<GetMeQuery.UserResponse>>
{
    public record UserResponse(Guid Id, string DiscordId, string Name, string? AvatarUrl, int Money);

    public sealed class Handler(IMainContext context, IUserProvider userProvider) : IRequestHandler<GetMeQuery, Result<UserResponse>>
    {
        public async Task<Result<UserResponse>> Handle(GetMeQuery request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == userProvider.UserId, cancellationToken);
            if (user is null) return new Error("Unauthorized", "User not found");

            return new UserResponse(user.Id, user.DiscordId.ToString(), user.Name, user.AvatarUrl, user.Money);
        }
    }
}