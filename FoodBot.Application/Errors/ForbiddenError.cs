using MyResult;

namespace FoodBot.Application.Errors;

public class ForbiddenError(ForbiddenError.ErrorCode errorCode) : Error(errorCode.ToString(), $"Forbidden. {errorCode}")
{
    public enum ErrorCode
    {
        RootRequired,
        IllegalAction,
        NoFunds,
        TransactionLimitReached,
        IncompleteOrder
    }
}
