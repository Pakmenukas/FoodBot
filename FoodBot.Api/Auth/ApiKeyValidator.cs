using MyResult;

namespace FoodBot.Api.Auth;

public sealed class ApiKeyValidator
{
    public Result ValidateAsync(string id, string secret)
    {
        return secret == "test-secret" // TODO extract to config
            ? Result.Ok() 
            : Result.Fail(new Error("Auth", "Invalid API key."));
    }
}