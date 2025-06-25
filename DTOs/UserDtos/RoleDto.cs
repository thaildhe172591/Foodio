namespace FoodioAPI.DTOs.UserDtos
{
    /// <summary>
    /// DTO cho thông tin vai trò
    /// Chỉ trả về thông tin cần thiết, không expose sensitive data
    /// </summary>
    public class RoleDto
    {
        /// <summary>
        /// ID của vai trò
        /// </summary>
        public string Id { get; set; } = default!;

        /// <summary>
        /// Tên vai trò (tên gốc trong hệ thống)
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Tên hiển thị của vai trò
        /// </summary>
        public string DisplayName { get; set; } = default!;

        /// <summary>
        /// Mô tả vai trò
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Có phải vai trò hệ thống không (không thể xóa/sửa)
        /// </summary>
        public bool IsSystemRole { get; set; }
    }
} 