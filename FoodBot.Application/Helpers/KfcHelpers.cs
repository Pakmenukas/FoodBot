namespace FoodBot.Application.Helpers
{
    public static class KfcHelpers
    {
        public static string ConstructKfcOrderString(string taste, string drink, string extraItems) => 
            $"{taste} akcijinis su {drink}{(string.IsNullOrEmpty(extraItems) ? "" : " + " + extraItems)}";
    }
}
