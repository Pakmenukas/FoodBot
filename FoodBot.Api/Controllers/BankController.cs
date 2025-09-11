using FoodBot.Application.Bank;
using FoodBot.Application.Errors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodBot.Api.Controllers;

[Route("api/bank")]
[ApiController]
[Authorize]
public sealed class BankController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetBalanceAll()
    {
        var result = await mediator.Send(new GetBalanceAllQuery(123));
        if (result.IsFailure)
        {
            return result.Error switch
            {
                NotFoundError => NotFound(),
                ForbiddenError => Forbid(),
                _ => BadRequest()
            };
        }

        return Ok(result.Value);
    }
    
    [HttpPost("add")]
    public async Task<ActionResult> AddMoney(AddMoneyToUserCommand command)
    {
        var result = await mediator.Send(command);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        
        return NoContent();   
    }
}