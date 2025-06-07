using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.DTOs.Auth
{
    public class ForgotPasswordDTO
    {
        [Required]
        public string Email { get; set; } = String.Empty;
    }
}