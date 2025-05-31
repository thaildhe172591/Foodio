using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.Entities;

public class OrderItem
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "OrderId is required.")]
    public Guid OrderId { get; set; }

    [Required(ErrorMessage = "MenuItemId is required.")]
    public Guid MenuItemId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    [MaxLength(255, ErrorMessage = "Note must be at most 255 characters.")]
    public string? Note { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; } = default!;
    public virtual MenuItem MenuItem { get; set; } = default!;
    public virtual ICollection<OrderItemStatusHistory> StatusHistories { get; set; } = new List<OrderItemStatusHistory>();
}
