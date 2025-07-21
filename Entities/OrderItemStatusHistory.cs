using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FoodioAPI.Entities.Abstractions;

namespace FoodioAPI.Entities;

public class OrderItemStatusHistory : Entity
{
    //public Guid Id { get; set; }

    [Required(ErrorMessage = "OrderItemId is required.")]
    public Guid OrderItemId { get; set; }

    [Required(ErrorMessage = "StatusId is required.")]
    public Guid StatusId { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual OrderItem OrderItem { get; set; } = default!;


    [ForeignKey(nameof(StatusId))]
    public virtual OrderItemStatus OrderItemStatus { get; set; } = default!;
}
