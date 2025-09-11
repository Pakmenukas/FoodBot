using FoodBot.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyResult;

namespace FoodBot.Application.User;

public sealed class GetMeQuery : IRequest<Result<Domain.User>>
{
    public sealed class Handler(IMainContext context, IUserProvider userProvider) : IRequestHandler<GetMeQuery, Result<Domain.User>>
    {
        public async Task<Result<Domain.User>> Handle(GetMeQuery request, CancellationToken cancellationToken)
        {
            var user = await context.Users.FirstOrDefaultAsync(e => e.Id == userProvider.UserId, cancellationToken);
            if (user is null) return new Error("Unauthorized", "User not found");

            return user;
        }
    }
}