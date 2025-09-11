using FoodBot.Application.Errors.Common;

namespace FoodBot.Application.Errors;

public class BadRequestError(BadRequestError.ErrorCode errorCode) : BaseError<BadRequestError.ErrorCode>(errorCode, MessageMap), IMessageMap<BadRequestError.ErrorCode>
{

    public enum ErrorCode
    {
        AmountTooLow,
        InvalidAmountNumberFormat,
    }

    public static Dictionary<ErrorCode, string> MessageMap { get; } = new()
    {
        { ErrorCode.AmountTooLow, "Per maža suma" },
        { ErrorCode.InvalidAmountNumberFormat, "Netaisyklingas sumos formatas" },
    };

}
