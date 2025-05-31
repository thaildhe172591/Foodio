using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.Entities;

public class TableOrder
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "TableId is required.")]
    public Guid TableId { get; set; }

    [Required(ErrorMessage = "OrderId is required.")]
    public Guid OrderId { get; set; }

    // Navigation properties
    public virtual DiningTable DiningTable { get; set; } = default!;
    public virtual Order Order { get; set; } = default!;
}
