using MyResult;

namespace FoodBot.Application.Errors;

public class BadRequestError(BadRequestError.ErrorCode errorCode) : Error(errorCode.ToString(), $"BadRequest. {errorCode}")
{
    public enum ErrorCode
    {
        AmountTooLow
    }
}
