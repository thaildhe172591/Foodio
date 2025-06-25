using System.ComponentModel.DataAnnotations;
using FoodioAPI.Entities.Abstractions;

namespace FoodioAPI.Entities;

public class MenuItem : Entity
{
    [Required(ErrorMessage = "Menu item name is required.")]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = default!;

    [MaxLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
    public string? Description { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative number.")]
    public decimal Price { get; set; }

    [MaxLength(255, ErrorMessage = "Image URL cannot exceed 255 characters.")]
    public string? ImageUrl { get; set; }

    public Guid CategoryId { get; set; }

    // Trạng thái còn món/hết món
    public bool IsAvailable { get; set; } = true;

    // Navigation
    public virtual Category Category { get; set; } = default!;
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
