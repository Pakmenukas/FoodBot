namespace DiscordService;

public sealed class DiscordOptions
{
    public required string DiscordApplicationId { get; init; }
    public required string DiscordSecret { get; init; }
    public required string RedirectUrl { get; init; }
    public required string GuildId { get; init; }
}