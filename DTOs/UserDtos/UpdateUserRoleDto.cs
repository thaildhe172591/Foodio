using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.DTOs.UserDtos
{
    public class UpdateUserRoleDto
    {
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = default!;
    }
}
