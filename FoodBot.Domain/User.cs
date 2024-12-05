using System.ComponentModel.DataAnnotations;
using FoodBot.Domain.Base;
using FoodBot.Domain.Enums;

namespace FoodBot.Domain;

public class User : BaseEntity<Guid>
{
    [Required]
    public ulong DiscordId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public int Money { get; set; }

    [Required]
    public bool NoGarbage { get; set; }

    [Required]
    public Role Role { get; set; }
}
