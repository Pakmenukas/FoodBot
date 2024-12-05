using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FoodBot.Domain.Base;

namespace FoodBot.Domain;

public class Log : BaseEntity<Guid>
{
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    public Guid UserId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public string Command { get; set; }

    [Required]
    public bool Success { get; set; }


    [Required]
    public string Data { get; set; }
}
