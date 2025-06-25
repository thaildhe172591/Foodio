namespace FoodioAPI.DTOs.UserDtos;

public class UpdateUserDto
{
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? UserName { get; set; }
    /// <summary>
    /// Danh sách RoleIds mới (tùy chọn). Nếu null hoặc rỗng sẽ không thay đổi quyền.
    /// </summary>
    public List<string>? RoleIds { get; set; }
} 