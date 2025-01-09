using FoodBot.Application.Errors.Common;

namespace FoodBot.Application.Errors;

public class ForbiddenError(ForbiddenError.ErrorCode errorCode) : BaseError<ForbiddenError.ErrorCode>(errorCode, MessageMap), IMessageMap<ForbiddenError.ErrorCode>
{
    public enum ErrorCode
    {
        RootRequired,
        IllegalAction,
        NoFunds,
        TransactionLimitReached,
        IncompleteOrder,
        CantTransferMoneyToSelf
    }

    public static Dictionary<ErrorCode, string> MessageMap { get; } = new()
    {
        { ErrorCode.RootRequired, "Ne administratorius" },
        { ErrorCode.IllegalAction, "Nelegalus veiksmas" },
        { ErrorCode.NoFunds, "Per mažai turima pinigo" },
        { ErrorCode.TransactionLimitReached, "Per didelė suma" },
        { ErrorCode.IncompleteOrder, "Užsakymas dar nepabaigtas" },
        { ErrorCode.CantTransferMoneyToSelf, "Negalima sau perversti pinigų" }
    };
}
