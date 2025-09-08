using FoodBot.Application;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodBot.Api.Controllers;

[Route("api/auth")]
[ApiController]
public sealed class AuthController(ISender mediator) : ControllerBase
{
    [HttpGet("test")]
    [Authorize]
    public async Task<ActionResult> Test()
    {
        var result = await mediator.Send(new TestQuery());
        if (result.IsFailure)
        {
            return BadRequest($"{result.Error.Code} {result.Error.Description}");
        }
        foreach (var userClaim in HttpContext.User.Claims)
        {
            Console.WriteLine($"{userClaim.Type} {userClaim.Value}");
        }

        return Ok(result.Value);
    }
}