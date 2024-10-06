using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Vision.Data.Models;
public class Panel : Entity
{
    [MaxLength(200)]
    public required string Name { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
    public string? Color { get; set; }

    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
}
