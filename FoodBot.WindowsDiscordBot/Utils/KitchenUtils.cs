namespace FoodBot.WindowsDiscordBot.Utils
{
    public static class KitchenUtils
    {
        public static string GetRandomFood()
        {
            List<string> list = new List<string>
        {
            ":carrot:",
            ":potato:",
            ":garlic:",
            ":onion:",
            ":croissant:",
            ":bagel:",
            ":bread:",
            ":french_bread:",
            ":flatbread:",
            ":pretzel:",
            ":cheese:",
            ":cooking:",
            ":pancakes:",
            ":waffle:",
            ":bacon:",
            ":cut_of_meat:",
            ":poultry_leg:",
            ":meat_on_bone:",
            ":hotdog:",
            ":hamburger:",
            ":fries:",
            ":pizza:",
            ":sandwich:",
            ":stuffed_flatbread:",
            ":taco:",
            ":burrito:",
            ":tamale:",
            ":salad:",
            ":shallow_pan_of_food:",
            ":fondue:",
            ":canned_food:",
            ":spaghetti:",
            ":ramen:",
            ":stew:",
            ":curry:",
            ":sushi:",
            ":bento:",
            ":dumpling:",
            ":oyster:",
            ":rice_ball:",
            ":rice:",
        };
            Random random = new Random();
            return list[random.Next(list.Count)];
        }
    }
}
