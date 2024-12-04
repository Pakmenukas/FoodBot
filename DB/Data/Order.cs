using DB.Data.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Data;

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
