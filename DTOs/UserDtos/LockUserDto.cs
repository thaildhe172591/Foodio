using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.DTOs.UserDtos
{
    public class LockUserDto
    {
        [Required]
        [Range(1, 365, ErrorMessage = "Lockout days must be between 1 and 365")]
        public int LockoutDays { get; set; }
    }
} 