using FoodBot.Application.Common;

namespace FoodBot.Api.Auth;

public sealed class UserProvider(IHttpContextAccessor httpContextAccessor) : IUserProvider
{
    public Guid? UserId => GetUserId();

    private Guid? GetUserId()
    {
        var result = Guid.TryParse(GetClaimValue(), out var userId);
        return result ? userId : null;
    }

    private string GetClaimValue()
    {
        return httpContextAccessor.HttpContext!.User.Claims
            .Single(claim => claim.Type == "UserId") // TODO: move to config
            .Value;
    }
}