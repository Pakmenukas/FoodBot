namespace FoodBot.Application.Errors.Common
{
    public interface IMessageMap<TEnum> where TEnum : Enum
    {
        static abstract Dictionary<TEnum, string> MessageMap { get; }
    }
}
