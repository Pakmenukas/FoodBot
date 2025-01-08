using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FoodBot.Domain.Base;

namespace FoodBot.Domain;

public class Purchase : BaseEntity<Guid>
{
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    public Guid UserId { get; set; }

    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; }
    public Guid OrderId { get; set; }

    [Required]
    public int Money { get; set; }

    [Required]
    public string Product { get; set; }

    [Required]
    public DateTime Date { get; set; } = DateTime.Now;


    // temporary
    [NotMapped]
    public int ChanceInt { get; set; }
}
