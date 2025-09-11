using FoodBot.Application.Bank;
using FoodBot.Application.Discord.Bank;
using FoodBot.Application.Errors;
using FoodBot.Application.Kitchen;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AddMoneyCommand = FoodBot.Application.Bank.AddMoneyCommand;

namespace FoodBot.Api.Controllers;

[Route("api/kitchen")]
[ApiController]
[Authorize]
public sealed class KitchenController(ISender mediator) : ControllerBase
{
    [HttpPost("idrink")]
    public async Task<ActionResult> PurchaseDrink()
    {
        var result = await mediator.Send(new PurchaseDrinkCommand());
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
    
    [HttpGet("drinks/my")]
    public async Task<ActionResult> GetDrinkCount()
    {
        var result = await mediator.Send(new DrinkCountQuery());
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
    
    [HttpGet("drinks/leaderboard")]
    public async Task<ActionResult> GetDrinkLeaderboard()
    {
        var result = await mediator.Send(new DrinksLeaderboardQuery());
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}