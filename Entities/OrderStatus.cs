using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.Entities;

public class OrderStatus
{
    public Guid Id { get; set; }

    [MaxLength(50)]
    public string Code { get; set; } = default!;

    [MaxLength(100)]
    public string Name { get; set; } = default!;

    // Navigation
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
