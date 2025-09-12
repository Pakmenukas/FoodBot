namespace DiscordService.Responses;

public sealed class UserResponse
{
    public string Id { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string? Avatar { get; init; } = string.Empty;
    public string? GlobalName { get; init; } = string.Empty;
}