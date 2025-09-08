using FoodBot.Application.Bank;
using FoodBot.Application.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FoodBot.Api.Controllers;

[Route("api/bank")]
[ApiController]
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
}