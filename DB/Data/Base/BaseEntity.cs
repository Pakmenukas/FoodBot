using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DB.Data.Base;

public class BaseEntity<T>
{
    [Key]
    public T Id { get; set; }
}
