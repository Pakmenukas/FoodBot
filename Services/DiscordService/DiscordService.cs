using System.Text.Json;
using DiscordService.Responses;
using FoodBot.Application.Common;
using FoodBot.Domain;
using MyResult;

namespace DiscordService;

public sealed class DiscordService(IDiscordToken discordToken) : IDiscord
{
    public async Task<Result<User>> GetUser(ulong userId, CancellationToken cancellationToken = default)
    {
        using HttpClient client = new();
        Console.WriteLine(userId);
        client.BaseAddress = new Uri($"https://discord.com/api/v10/");
        client.DefaultRequestHeaders.Add("Authorization", $"Bot {discordToken.Token}");
        
        var response = await client.GetAsync($"users/{userId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return new Error(response.StatusCode.ToString(), "Could not get user");
        }
        
        var userJson = await response.Content.ReadAsStringAsync(cancellationToken);
        Console.WriteLine(userJson);
        var user = JsonSerializer.Deserialize<UserResponse>(userJson, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
        if (user is null)
        {
            return new Error("JSON", "Could not deserialize user");
        }
        
        return Result<User>.Ok(new User
        {
            DiscordId = ulong.Parse(user.Id),
            Name = user.GlobalName,
            AvatarUrl = $"https://cdn.discordapp.com/avatars/{user.Id}/{user.Avatar}.png",
        });
    }
}