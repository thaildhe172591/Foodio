using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.Entities;

public class OrderDeliveryInfo
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "OrderId is required.")]
    public Guid OrderId { get; set; }

    [Required(ErrorMessage = "Receiver name is required.")]
    [MaxLength(100, ErrorMessage = "Receiver name must be at most 100 characters.")]
    public string ReceiverName { get; set; } = default!;

    [Required(ErrorMessage = "Receiver phone is required.")]
    [MaxLength(20, ErrorMessage = "Receiver phone must be at most 20 characters.")]
    public string ReceiverPhone { get; set; } = default!;

    [Required(ErrorMessage = "Delivery address is required.")]
    [MaxLength(255, ErrorMessage = "Delivery address must be at most 255 characters.")]
    public string DeliveryAddress { get; set; } = default!;

    // Navigation property
    public virtual Order Order { get; set; } = default!;
}
