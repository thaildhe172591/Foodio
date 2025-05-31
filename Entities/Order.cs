using System.ComponentModel.DataAnnotations;
using FoodioAPI.Entities;

namespace FoodioAPI.Entities;

public class Order
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "UserId is required.")]
    public string UserId { get; set; } = default!;

    public Guid? TableId { get; set; }

    [Required(ErrorMessage = "OrderTypeId is required.")]
    public Guid OrderTypeId { get; set; }

    [Required(ErrorMessage = "Order status is required.")]
    public Guid StatusId { get; set; }

    [Required(ErrorMessage = "Total amount is required.")]
    public decimal Total { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User User { get; set; } = default!;
    public virtual DiningTable? DiningTable { get; set; }
    public virtual OrderType OrderType { get; set; } = default!;
    public virtual OrderStatus Status { get; set; } = default!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual OrderDeliveryInfo? DeliveryInfo { get; set; }
    public virtual ICollection<OrderShipper> OrderShippers { get; set; } = new List<OrderShipper>();
    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
    public virtual ICollection<TableOrder> TableOrders { get; set; } = new List<TableOrder>();

}
