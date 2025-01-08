namespace FoodBot.WindowsDiscordBot.Utils;

public static class UserStatusUtils
{
    public static string GetUserMoneyStatusEmoji(int moneyAmount)
    {
        // if (index == 0) return ":trophy:";
        return moneyAmount switch
        {
            <= -2000 => ":money_with_wings: :interrobang:",
            0 => ":shaking_face:",
            < 0 => ":money_with_wings:",
            _ => ""
        };
    }
}