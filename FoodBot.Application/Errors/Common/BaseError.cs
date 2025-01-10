using MyResult;

namespace FoodBot.Application.Errors.Common
{
    public abstract class BaseError<TEnum>(
        TEnum errorCode,
        Dictionary<TEnum, string> messageMap
    ) : Error(
        errorCode.ToString(),
        messageMap.ContainsKey(errorCode)
            ? $":exclamation:{messageMap[errorCode]}:exclamation:"
            : $"Klaida: {errorCode}" 
    ) where TEnum : Enum
    {
    }
}
