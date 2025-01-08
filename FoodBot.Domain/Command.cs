using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FoodBot.Domain.Base;

namespace FoodBot.Domain;

public class Command : BaseEntity<Guid>
{
    [ForeignKey(nameof(ToUserId))]
    public User ToUser { get; set; }
    public Guid ToUserId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public int Money { get; set; }

    [Required]
    public int Count { get; set; }
}
