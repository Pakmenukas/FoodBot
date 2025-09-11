using FoodBot.Application.Bank;
using FoodBot.Application.Common;
using FoodBot.Application.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodBot.Api.Controllers;

[Route("api/user")]
[ApiController]
[Authorize]
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
    
    [HttpPost("add")]
    public async Task<ActionResult> AddUser(AddUserCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }
    
    [HttpGet("all")]
    public async Task<ActionResult> GetUserAllList(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllUserListQuery(), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
    
    [HttpDelete("{userId:guid}")]
    public async Task<ActionResult> DeleteUser([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteUserCommand(userId), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }
    
    [HttpDelete]
    public async Task<ActionResult> DeleteMe(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteMeCommand(), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }
}