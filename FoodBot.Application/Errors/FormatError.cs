using MyResult;

namespace FoodBot.Application.Errors;

public class FormatError(string description) : Error("Format", description);
