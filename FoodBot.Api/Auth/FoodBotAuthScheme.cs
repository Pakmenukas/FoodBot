namespace FoodBot.Api.Auth;

public class FoodBotAuthScheme
{
    public const string DefaultScheme = "Combined";
    public const string DisplayName = "Combined Cookie or ApiKey";

    public const string ApiKeyScheme = "ApiKey";
    public const string ApiKeySchemeHeader = "X-Api-Key";
    public const string ApiKeyPrefix = "ApiKey";
}