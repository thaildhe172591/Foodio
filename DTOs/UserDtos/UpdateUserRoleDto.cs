using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.DTOs.UserDtos
{
    /// <summary>
    /// DTO để cập nhật vai trò người dùng
    /// Sử dụng Role IDs thay vì Role Names để bảo mật hơn
    /// </summary>
    public class UpdateUserRoleDto
    {
        /// <summary>
        /// Danh sách ID của các vai trò cần gán cho người dùng
        /// </summary>
        [Required(ErrorMessage = "Danh sách vai trò không được để trống")]
        [MinLength(1, ErrorMessage = "Phải có ít nhất 1 vai trò")]
        public List<string> RoleIds { get; set; } = new List<string>();
    }
}
