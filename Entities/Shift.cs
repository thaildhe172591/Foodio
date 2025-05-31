using System.ComponentModel.DataAnnotations;
using FoodioAPI.Entities;

namespace FoodioAPI.Entities;

public class Shift
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = default!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    [MaxLength(50)]
    public string Role { get; set; } = default!;

    public virtual User User { get; set; } = default!;
}
