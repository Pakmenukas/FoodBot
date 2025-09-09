using FoodBot.Application.Common;
using Microsoft.Extensions.Options;

namespace FoodBot.Infrastructure;

public sealed class DiscordTokenProvider(IOptions<AppOptions> dataOptions) : IDiscordToken
{
    private string? _token;

    public string Token => GetToken();

    private string GetToken()
    {
        if (_token is not null) return _token!;
        _token = dataOptions.Value.DiscordToken;
        return dataOptions.Value.DiscordToken;
    }
}