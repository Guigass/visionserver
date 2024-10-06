using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Data.Models;
public class User : IdentityUser<Guid>
{
    [MaxLength(200)]
    public string? FirstName { get; set; }
    [MaxLength(200)]
    public string? LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public bool IsActive { get; set; }
    public DateTime LastLogin { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
