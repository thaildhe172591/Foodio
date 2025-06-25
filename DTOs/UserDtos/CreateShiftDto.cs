using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.DTOs.UserDtos
{
    public class CreateShiftDto
    {
        [Required]
        public string UserId { get; set; } = default!;
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required, MaxLength(50)]
        public string Role { get; set; } = default!;
    }
}
