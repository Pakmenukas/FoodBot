using DB.Data.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Data;

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
