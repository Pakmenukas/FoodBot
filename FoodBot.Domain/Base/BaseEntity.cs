using System.ComponentModel.DataAnnotations;

namespace FoodBot.Domain.Base;

public class BaseEntity<T> where T : struct
{
    [Key]
    public T Id { get; set; }
}
