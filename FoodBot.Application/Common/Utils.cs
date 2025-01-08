namespace FoodBot.Application.Common
{
    public static class Utils
    {
        public static int GetRandomNumber(int max)
        {
            string result = "";
            try
            {
                using (var client = new HttpClient())
                {
                    result = client.GetStringAsync($"https://www.random.org/integers/?num={1}&min={1}&max={max - 1}&col={1}&base={10}&format=plain&rnd=new").Result;
                }
                if (int.TryParse(result, out int integer))
                {
                    return integer;
                }
            }
            catch
            {
            }

            Random random = new Random();
            return random.Next(max);
        }
    }
}
