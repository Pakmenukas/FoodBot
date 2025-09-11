namespace DiscordService.Responses;

public sealed class GuildMembersResponse
{
    public string? Nick { get; init; }
    public string? Avatar { get; init; }
    public UserResponse User { get; init; } = new();
}