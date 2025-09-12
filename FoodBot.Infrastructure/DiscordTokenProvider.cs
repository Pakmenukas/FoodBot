using FoodBot.Application.Common;
using Microsoft.Extensions.Options;

namespace FoodBot.Infrastructure;

public sealed class DiscordTokenProvider(IOptions<AppOptions> dataOptions) : IDiscordToken
{
    public string Token => dataOptions.Value.DiscordToken;
}