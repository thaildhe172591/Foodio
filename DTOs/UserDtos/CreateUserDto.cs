using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.DTOs.UserDtos
{
    /// <summary>
    /// DTO để tạo người dùng mới
    /// </summary>
    public class CreateUserDto
    {
        /// <summary>
        /// Email của người dùng (sẽ dùng làm username)
        /// </summary>
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = default!;

        /// <summary>
        /// Mật khẩu của người dùng
        /// </summary>
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6-100 ký tự")]
        public string Password { get; set; } = default!;

        /// <summary>
        /// Số điện thoại của người dùng (không bắt buộc)
        /// </summary>
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được quá 20 ký tự")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Danh sách ID của các vai trò cần gán cho người dùng
        /// </summary>
        [Required(ErrorMessage = "Danh sách vai trò không được để trống")]
        [MinLength(1, ErrorMessage = "Phải có ít nhất 1 vai trò")]
        public List<string> RoleIds { get; set; } = new List<string>();

        /// <summary>
        /// Có xác nhận email ngay không (mặc định: false)
        /// </summary>
        public bool EmailConfirmed { get; set; } = false;

        /// <summary>
        /// Có xác nhận số điện thoại ngay không (mặc định: false)
        /// </summary>
        public bool PhoneNumberConfirmed { get; set; } = false;
    }
} 