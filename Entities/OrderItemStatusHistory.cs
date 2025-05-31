using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.Entities;

public class OrderItemStatusHistory
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "OrderItemId is required.")]
    public Guid OrderItemId { get; set; }

    [Required(ErrorMessage = "StatusId is required.")]
    public Guid StatusId { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual OrderItem OrderItem { get; set; } = default!;
    public virtual OrderItemStatus Status { get; set; } = default!;
}
