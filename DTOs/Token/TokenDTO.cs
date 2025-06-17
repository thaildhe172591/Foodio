using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.DTOs.Token;

public class TokenDTO
{
    [Required]
    public string AccessToken { get; set; } = string.Empty;
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
