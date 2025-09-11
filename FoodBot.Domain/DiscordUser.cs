namespace FoodBot.Domain;

public class DiscordUser
{
    public ulong Id { get; set; }

    public string Name { get; set; }

    public string? AvatarUrl { get; set; }
}
