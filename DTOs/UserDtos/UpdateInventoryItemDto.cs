using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.DTOs.UserDtos
{
    public class UpdateInventoryItemDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;
        [Required, Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        [Required, MaxLength(50)]
        public string Unit { get; set; } = default!;
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
