using FoodBot.Application.Common;

namespace FoodBot.Infrastructure;

public sealed class DiscordTokenProvider : IDiscordToken
{
    private string? _token = null;

    public string Token => GetToken();

    private string GetToken()
    {
        if (_token is not null) return _token!;

        var tokenPath = Path.Combine(AppContext.BaseDirectory, "Data", "token.txt");
        var token = File.ReadAllText(tokenPath);
        _token = token;
        return token;
    }
}