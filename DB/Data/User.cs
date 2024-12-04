using DB.Data.Base;
using DB.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Data;

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
