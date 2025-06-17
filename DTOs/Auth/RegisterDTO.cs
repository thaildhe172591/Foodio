using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.DTOs.Auth;

public class RegisterDTO
{
    [Required(ErrorMessage = "Vui lòng nhập Email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
        ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Vui lòng xác nhận Mật khẩu")]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmPassword { get; set; }
}
