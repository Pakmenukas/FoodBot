using MyResult;

namespace FoodBot.Application.Errors;

public class NotFoundError(NotFoundError.ErrorCode errorCode) : Error(errorCode.ToString(), $"{errorCode} not found")
{
    public enum ErrorCode
    {
        Initiator,
        TargetUser,
        Order,
        Purchase
    }
}
