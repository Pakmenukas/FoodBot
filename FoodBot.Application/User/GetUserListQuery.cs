using FoodBot.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyResult;

namespace FoodBot.Application.User;

public sealed class GetUserListQuery : IRequest<Result<List<GetUserListQuery.UserResponse>>>
{
    public record UserResponse(Guid? Id, string DiscordId, string Name, string? AvatarUrl, int Money);

    public sealed class Handler(IMainContext context, IDiscord discord, ILogger<GetUserListQuery> logger) 
        : IRequestHandler<GetUserListQuery, Result<List<UserResponse>>>
    {
        public async Task<Result<List<UserResponse>>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
        {
            var discordUsers = await discord.GetMembers(cancellationToken);
            var savedUsers = await context.Users
                .AsNoTracking()
                .Select(c => new UserResponse(c.Id, c.DiscordId.ToString(), c.Name, c.AvatarUrl, c.Money))
                .ToListAsync(cancellationToken);

            // Combine and deduplicate by DiscordId
            List<UserResponse> combined;
            if (discordUsers.IsSuccess)
            {
                combined = savedUsers
                    .Concat(discordUsers.Value.Select(c => new UserResponse(null, c.Id.ToString(), c.Name, c.AvatarUrl, 0)))
                    .GroupBy(u => u.DiscordId)
                    .Select(g => g.First())
                    .ToList();
            }
            else
            {
                logger.LogWarning(discordUsers.Error.ToString());
                combined = savedUsers
                    .GroupBy(u => u.DiscordId)
                    .Select(g => g.First())
                    .ToList();
            }
            
            return combined;
        }
    }
}