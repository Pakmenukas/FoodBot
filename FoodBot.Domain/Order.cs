using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FoodBot.Domain.Base;

namespace FoodBot.Domain;

public class Order : BaseEntity<Guid>
{
    [Required]
    public DateTime DateCreated { get; set; } = DateTime.Now;

    public DateTime DateCompleted { get; set; } = DateTime.Now;

    [ForeignKey(nameof(GarbagePersonId))]
    public User GarbagePerson { get; set; }
    public Guid? GarbagePersonId { get; set; }

    [Required]
    public bool IsComplete { get; set; }
    

    // to many
    public List<Purchase> PurchaseList { get; } = new();
}
