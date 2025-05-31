using System.ComponentModel.DataAnnotations;
using FoodioAPI.Entities;

namespace FoodioAPI.Entities;

public class OrderType
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Order type code is required.")]
    [MaxLength(50, ErrorMessage = "Order type code cannot exceed 50 characters.")]
    public string Code { get; set; } = null!;

    [Required(ErrorMessage = "Order type name is required.")]
    [MaxLength(100, ErrorMessage = "Order type name cannot exceed 100 characters.")]
    public string Name { get; set; } = null!;

    // Navigation
    public ICollection<Order>? Orders { get; set; }
}
