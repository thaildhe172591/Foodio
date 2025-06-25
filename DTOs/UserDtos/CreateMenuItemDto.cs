using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.DTOs.UserDtos
{
    public class CreateMenuItemDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;
        [MaxLength(255)]
        public string? Description { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        [MaxLength(255)]
        public string? ImageUrl { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
