using System.Globalization;
using System.Text.RegularExpressions;
using FoodBot.Application.Errors;
using MyResult;

namespace FoodBot.Application.Common;

public static class MoneySerializer
{
    public static Result<int> Deserialize(object obj)
    {
        var str = obj.ToString()!.Replace('.', ',');
        if (!Regex.IsMatch(str, @"^[+-]?\d{1,3}([,.]\d{1,2})?$"))
            return new FormatError("Failed to serialize money");

        // 4,80 * 100 = 480,0003
        var money = int.Parse(Math.Round((float.Parse(str) * 100)).ToString(CultureInfo.InvariantCulture));

        return money;
    }

    public static string Serialize(int money)
    {
        return $"{(float)money / 100:#0.00}";
    }
}