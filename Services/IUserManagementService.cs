using FoodioAPI.DTOs;
using FoodioAPI.DTOs.UserDtos;

namespace FoodioAPI.Services;

/// <summary>
/// Interface cho User Management Service
/// Định nghĩa các phương thức quản lý người dùng cho Admin
/// </summary>
public interface IUserManagementService
{
    /// <summary>
    /// Lấy danh sách người dùng với phân trang và tìm kiếm cơ bản
    /// </summary>
    /// <param name="page">Số trang</param>
    /// <param name="pageSize">Số lượng item trên mỗi trang</param>
    /// <param name="searchTerm">Từ khóa tìm kiếm</param>
    /// <returns>Danh sách người dùng với thông tin phân trang</returns>
    Task<PaginatedData<UserDto>> GetUsersAsync(int page = 1, int pageSize = 10, string? searchTerm = null);

    /// <summary>
    /// Lấy danh sách người dùng với các bộ lọc chi tiết
    /// </summary>
    /// <param name="searchDto">Các tham số tìm kiếm và lọc</param>
    /// <returns>Danh sách người dùng với thông tin phân trang</returns>
    Task<PaginatedData<UserDto>> Search(UserSearchDto searchDto, string roleNotQuerry = "");

    /// <summary>
    /// Lấy thông tin chi tiết của một người dùng theo ID
    /// </summary>
    /// <param name="id">ID của người dùng</param>
    /// <returns>Thông tin người dùng hoặc null nếu không tìm thấy</returns>
    Task<UserDto?> GetUserByIdAsync(string id);

    /// <summary>
    /// Lấy thông tin chi tiết của một người dùng theo email
    /// </summary>
    /// <param name="email">Email của người dùng</param>
    /// <returns>Thông tin người dùng hoặc null nếu không tìm thấy</returns>
    Task<UserDto?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Cập nhật vai trò của người dùng
    /// Xóa tất cả vai trò hiện tại và gán vai trò mới
    /// Sử dụng Role IDs để bảo mật hơn
    /// </summary>
    /// <param name="userId">ID của người dùng cần cập nhật</param>
    /// <param name="roleIds">Danh sách ID của các vai trò mới</param>
    /// <returns>True nếu cập nhật thành công</returns>
    Task<bool> UpdateUserRoleAsync(string userId, List<string> roleIds);

    /// <summary>
    /// Xóa người dùng khỏi hệ thống
    /// Không thể xóa tài khoản system hoặc admin
    /// </summary>
    /// <param name="userId">ID của người dùng cần xóa</param>
    /// <returns>True nếu xóa thành công</returns>
    Task<bool> DeleteUserAsync(string userId);

    /// <summary>
    /// Khóa tài khoản người dùng trong một khoảng thời gian nhất định
    /// </summary>
    /// <param name="userId">ID của người dùng cần khóa</param>
    /// <param name="lockoutDays">Số ngày khóa tài khoản</param>
    /// <returns>True nếu khóa thành công</returns>
    Task<bool> LockUserAsync(string userId, int lockoutDays);

    /// <summary>
    /// Mở khóa tài khoản người dùng ngay lập tức
    /// </summary>
    /// <param name="userId">ID của người dùng cần mở khóa</param>
    /// <returns>True nếu mở khóa thành công</returns>
    Task<bool> UnlockUserAsync(string userId);

    /// <summary>
    /// Kiểm tra trạng thái khóa của người dùng
    /// </summary>
    /// <param name="userId">ID của người dùng cần kiểm tra</param>
    /// <returns>True nếu tài khoản đang bị khóa, false nếu không</returns>
    Task<bool> IsUserLockedAsync(string userId);

    /// <summary>
    /// Lấy thời gian mở khóa của người dùng
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <returns>Thời gian mở khóa hoặc null nếu tài khoản không bị khóa</returns>
    Task<DateTimeOffset?> GetUserLockoutEndDateAsync(string userId);

    /// <summary>
    /// Lấy danh sách tất cả vai trò của một người dùng
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <returns>Danh sách tên vai trò</returns>
    Task<List<string>> GetUserRolesAsync(string userId);

    /// <summary>
    /// Lấy danh sách tất cả vai trò có trong hệ thống
    /// </summary>
    /// <returns>Danh sách vai trò với thông tin an toàn</returns>
    Task<List<RoleDto>> GetAllRolesAsync(string roleNot = "");

    /// <summary>
    /// Tạo người dùng mới
    /// </summary>
    /// <param name="dto">Thông tin người dùng cần tạo</param>
    /// <returns>Thông tin người dùng đã tạo</returns>
    Task<UserDto> CreateUserAsync(CreateUserDto dto);

    /// <summary>
    /// Cập nhật thông tin cơ bản của người dùng (email, phone, tên, vai trò...)
    /// </summary>
    /// <param name="userId">ID người dùng</param>
    /// <param name="dto">Thông tin cần cập nhật</param>
    /// <returns>True nếu thành công</returns>
    Task<bool> UpdateUserAsync(string userId, UpdateUserDto dto);
}