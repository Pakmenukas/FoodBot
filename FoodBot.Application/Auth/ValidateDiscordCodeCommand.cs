using MediatR;
using MyResult;

namespace FoodBot.Application.Auth;

public sealed class ValidateDiscordCodeCommand(string code)
    : IRequest<Result<Guid>>
{


    public sealed class Handler : IRequestHandler<ValidateDiscordCodeCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ValidateDiscordCodeCommand request, CancellationToken cancellationToken)
        {
            
            return Result.Ok(Guid.CreateVersion7());
        }
    }
}