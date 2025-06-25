using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.DTOs.Auth;

public class LoginDTO
{
    [Required(ErrorMessage = "Vui lòng nhập Email")]
    [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
    public string Password { get; set; }
}
