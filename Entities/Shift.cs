using FoodioAPI.Entities.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.Entities;

public class Shift : Entity
{
    public string UserId { get; set; } = default!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    [MaxLength(50)]
    public string Role { get; set; } = default!;

    public virtual User User { get; set; } = default!;
}
