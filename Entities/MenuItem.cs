using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.Entities;

public class MenuItem
{
    public Guid Id { get; set; }

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

    // Navigation
    public virtual Category Category { get; set; } = default!;
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
