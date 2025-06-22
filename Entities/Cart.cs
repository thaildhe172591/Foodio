using System.ComponentModel.DataAnnotations;
using FoodioAPI.Entities;

namespace FoodioAPI.Entities;

public class Cart
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "UserId is required.")]
    public string UserId { get; set; } = default!;

    public Guid? TableId { get; set; }

    [Required(ErrorMessage = "Type is required.")]
    [MaxLength(50, ErrorMessage = "Type cannot exceed 50 characters.")]
    public string Type { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsOrdered { get; set; } = false;

    // Navigation properties
    public virtual User User { get; set; } = default!;
    public virtual DiningTable? Table { get; set; }
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<OrderItem>? OrderItems { get; set; } = new List<OrderItem>();
}
