namespace FoodBot.Application.Common;

public sealed class AppOptions
{
    public required string DatabasePath { get; init; }
    public required string DiscordToken { get; init; }
}