using FoodBot.Application.Errors.Common;
namespace FoodBot.Application.Errors;

public class NotFoundError(NotFoundError.ErrorCode errorCode) : BaseError<NotFoundError.ErrorCode>(errorCode, MessageMap), IMessageMap<NotFoundError.ErrorCode>
{
    public enum ErrorCode
    {
        Initiator,
        TargetUser,
        Order,
        Purchase
    }

    public static Dictionary<ErrorCode, string> MessageMap { get; } = new()
    {
        { ErrorCode.Initiator, "Nėra tokio vartotojo" },
        { ErrorCode.TargetUser, "Nėra tokio vartotojo" },
        { ErrorCode.Order, "Nėra tokio užsakymo" },
        { ErrorCode.Purchase, "Nėra užsakymų" }
    };
}
