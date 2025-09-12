namespace FoodBot.Application.Common;

public interface IUserProvider
{
    public Guid? UserId { get; }
    // TODO: add permission level
}