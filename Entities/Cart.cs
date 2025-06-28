using System.ComponentModel.DataAnnotations;
using FoodioAPI.Entities;
using FoodioAPI.Entities.Abstractions;

namespace FoodioAPI.Entities;

public class Cart : Entity
{
    //public Guid Id { get; set; }

    //[Required(ErrorMessage = "UserId is required.")]
    public string UserId { get; set; }

    public Guid? TableId { get; set; }

    [Required(ErrorMessage = "Type is required.")]
    [MaxLength(50, ErrorMessage = "Type cannot exceed 50 characters.")]
    public string Type { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsOrdered { get; set; } = false;

    // Navigation properties
    public virtual User User { get; set; }
    public virtual DiningTable? Table { get; set; }
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

}
