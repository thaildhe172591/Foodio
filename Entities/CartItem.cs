using System.ComponentModel.DataAnnotations;
using FoodioAPI.Entities.Abstractions;

namespace FoodioAPI.Entities;

public class CartItem : Entity
{
    //public Guid Id { get; set; }

    [Required(ErrorMessage = "CartId is required.")]
    public Guid CartId { get; set; }

    [Required(ErrorMessage = "MenuItemId is required.")]
    public Guid MenuItemId { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    [MaxLength(255, ErrorMessage = "Note cannot exceed 255 characters.")]
    public string? Note { get; set; }

    // Navigation properties
    public virtual Cart Cart { get; set; } = default!;
    public virtual MenuItem MenuItem { get; set; } = default!;
}
