using FoodBot.Application.Common;
using Microsoft.Extensions.Options;

namespace FoodBot.Infrastructure;

public sealed class DiscordTokenProvider(IOptions<DataOptions> dataOptions) : IDiscordToken
{
    private string? _token;

    public string Token => GetToken();

    private string GetToken()
    {
        if (_token is not null) return _token!;

        var tokenPath = Path.Combine(dataOptions.Value.DataPath, "token.txt");
        var token = File.ReadAllText(tokenPath);
        _token = token;
        return token;
    }
}