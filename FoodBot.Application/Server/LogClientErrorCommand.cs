using FoodBot.Application.Common;
using MediatR;
using MyResult;

namespace FoodBot.Application.Server;

public class LogClientErrorCommand(ulong userId, string command, Error error) : IRequest<Result>
{
    private ulong UserId => userId;
    private string Command => command;
    private Error Error => error;

    public sealed class Handler(ILogger logger) : IRequestHandler<LogClientErrorCommand, Result>
    {
        public async Task<Result> Handle(LogClientErrorCommand request, CancellationToken cancellationToken)
        {
            await logger.LogError(request.UserId, request.Command, request.Error);

            return Result.Ok();
        }
    }
}