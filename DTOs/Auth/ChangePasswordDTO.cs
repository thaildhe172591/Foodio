using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.DTOs.Auth
{
    public class ChangePasswordDTO
    {
        [Required]
        public string Token { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}