using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FoodioAPI.Entities;

namespace FoodioAPI.Entities;

public class Delivery
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "OrderId is required.")]
    public Guid OrderId { get; set; }

    [Required(ErrorMessage = "ShipperId is required.")]
    public string? ShipperId { get; set; }

    [Required(ErrorMessage = "Fee is required.")]
    public decimal Fee { get; set; }

    [Required(ErrorMessage = "StatusId is required.")]
    public Guid StatusId { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; } = default!;
    public virtual User Shipper { get; set; } = default!;

    [ForeignKey(nameof(StatusId))]
    public virtual DeliveryStatus DeliveryStatus { get; set; } = default!;
}
