using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.Entities;

public class Category
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Category name is required.")]
    [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
    public string Name { get; set; } = default!;

    // Navigation
    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
