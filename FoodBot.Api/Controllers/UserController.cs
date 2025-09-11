using FoodBot.Application.Common;
using FoodBot.Application.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FoodBot.Api.Controllers;

[Route("api/user")]
[ApiController]
public sealed class UserController(ISender mediator, IUserProvider userProvider) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult> GetMe(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMeQuery(), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}