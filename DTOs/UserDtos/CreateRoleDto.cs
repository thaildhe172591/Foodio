using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.DTOs.UserDtos
{
    /// <summary>
    /// DTO để tạo vai trò mới
    /// Có validation để đảm bảo bảo mật
    /// </summary>
    public class CreateRoleDto
    {
        /// <summary>
        /// Tên vai trò (chỉ cho phép chữ cái, số và dấu gạch ngang)
        /// </summary>
        [Required(ErrorMessage = "Tên vai trò không được để trống")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên vai trò phải từ 2-50 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9\-_]+$", ErrorMessage = "Tên vai trò chỉ được chứa chữ cái, số, dấu gạch ngang và dấu gạch dưới")]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Tên hiển thị của vai trò
        /// </summary>
        [Required(ErrorMessage = "Tên hiển thị không được để trống")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên hiển thị phải từ 2-100 ký tự")]
        public string DisplayName { get; set; } = default!;

        /// <summary>
        /// Mô tả vai trò
        /// </summary>
        [StringLength(500, ErrorMessage = "Mô tả không được quá 500 ký tự")]
        public string? Description { get; set; }
    }
} 