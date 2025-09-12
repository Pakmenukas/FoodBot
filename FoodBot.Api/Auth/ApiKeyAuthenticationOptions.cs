using Microsoft.AspNetCore.Authentication;

namespace FoodBot.Api.Auth;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string DefaultScheme { get; set; } = string.Empty;
    public string ApiKeySchemeHeader { get; set; } = string.Empty;
    public string ApiKeyPrefix { get; set; } = string.Empty;
}