using System.ComponentModel.DataAnnotations;
using FoodioAPI.Entities;

namespace FoodioAPI.Entities;

public class OrderShipper
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "OrderId is required.")]
    public Guid OrderId { get; set; }

    [Required(ErrorMessage = "ShipperId is required.")]
    public string? ShipperId { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual Order Order { get; set; } = default!;
    public virtual User Shipper { get; set; } = default!;
}
