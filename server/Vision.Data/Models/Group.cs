using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Data.Models;
public class Group : Entity
{
    [MaxLength(200)]
    public required string Name { get; set; }          // Nome do grupo
    [MaxLength(500)]
    public required string Description { get; set; }   // Descrição do grupo
    public bool IsActive { get; set; }                 // Indica se o grupo está ativo
}
