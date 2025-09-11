using System.Security.Claims;
using FoodBot.Application.Auth;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodBot.Api.Controllers;

[Route("api/auth")]
[ApiController]
public sealed class AuthController(ISender mediator) : ControllerBase
{
    [HttpPost("discord/validate")]
    [AllowAnonymous]
    public async Task<ActionResult> ValidateDiscordCode([FromBody] string code)
    {
        var result = await mediator.Send(new ValidateDiscordCodeCommand(code));
        if (result.IsFailure)
        {
            return Unauthorized($"{result.Error.Code} {result.Error.Description}");
        }

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            AllowRefresh = true,
        };
        var identity = new ClaimsIdentity(
            [new Claim("userId", result.Value.ToString())],
            CookieAuthenticationDefaults.AuthenticationScheme,
            ClaimTypes.Name,
            ClaimTypes.Role);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            authProperties);

        return NoContent();
    }
    
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        
        return NoContent();
    }
    
    [HttpPost("oauth2/token")]
    [AllowAnonymous]
    public Task Test()
    {
        Console.WriteLine("headers" + HttpContext.Request.Headers.UserAgent);
        return Task.CompletedTask;
    }
}