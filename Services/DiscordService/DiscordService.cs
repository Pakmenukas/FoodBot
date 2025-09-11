using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DiscordService.Responses;
using FoodBot.Application.Common;
using FoodBot.Domain;
using Microsoft.Extensions.Options;
using MyResult;

namespace DiscordService;

public sealed class DiscordService(IDiscordToken discordToken, IOptions<DiscordOptions> options) : IDiscord
{
    private readonly Uri _baseUri = new("https://discord.com/api/v10/");
    
    public async Task<Result<User>> Authorize(string code, CancellationToken cancellationToken = default)
    {
        using HttpClient client = new();
        client.BaseAddress = _baseUri;
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{options.Value.DiscordApplicationId}:{options.Value.DiscordSecret}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        List<(string, string)> content =
        [
            ("grant_type", "authorization_code"),
            ("code", code),
            ("redirect_uri", Uri.EscapeDataString(options.Value.RedirectUrl))
        ];
          
        var contentString = string.Join("&", content.Select(x => $"{x.Item1}={x.Item2}"));
        
        var response = await client.PostAsync("oauth2/token", new StringContent(contentString, Encoding.UTF8, "application/x-www-form-urlencoded"), cancellationToken);
        if (!response.IsSuccessStatusCode) return new Error(
            response.StatusCode.ToString(), 
            await response.Content.ReadAsStringAsync(cancellationToken));

        var tokenJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var token = JsonSerializer.Deserialize<TokenResponse>(tokenJson, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
        if (token is null) return new Error("JSON", "Could not deserialize tokens");

        var userResult = await GetUser(token.AccessToken, cancellationToken);
        if (userResult.IsFailure) return userResult.Error;
        var user = userResult.Value;

        return Result<User>.Ok(new User
        {
            DiscordId = ulong.Parse(user.Id),
            Name = user.GlobalName,
            AvatarUrl = $"https://cdn.discordapp.com/avatars/{user.Id}/{user.Avatar}.png",
        });
    }

    public async Task<Result<User>> GetUser(ulong userId, CancellationToken cancellationToken = default)
    {
        using HttpClient client = new();
        client.BaseAddress = _baseUri;
        client.DefaultRequestHeaders.Add("Authorization", $"Bot {discordToken.Token}");
        
        var response = await client.GetAsync($"users/{userId}", cancellationToken);
        if (!response.IsSuccessStatusCode) return new Error(
            response.StatusCode.ToString(), 
            await response.Content.ReadAsStringAsync(cancellationToken));
        
        var userJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var user = JsonSerializer.Deserialize<UserResponse>(userJson, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
        if (user is null) return new Error("JSON", "Could not deserialize user");
        
        return Result<User>.Ok(new User
        {
            DiscordId = ulong.Parse(user.Id),
            Name = user.GlobalName,
            AvatarUrl = $"https://cdn.discordapp.com/avatars/{user.Id}/{user.Avatar}.png",
        });
    }

    private async Task<Result<UserResponse>> GetUser(string authToken, CancellationToken cancellationToken = default)
    {
        using HttpClient client = new();
        client.BaseAddress = _baseUri;
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");
        var response = await client.GetAsync($"users/@me", cancellationToken);
        if (!response.IsSuccessStatusCode) return new Error(
            response.StatusCode.ToString(), 
            await response.Content.ReadAsStringAsync(cancellationToken));
        
        var userJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var user = JsonSerializer.Deserialize<UserResponse>(userJson, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
        if (user is null) return new Error("JSON", "Could not deserialize user");
        
        return user;
    }
}