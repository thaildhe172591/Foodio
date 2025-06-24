using FoodioAPI.DTOs;
using FoodioAPI.DTOs.UserDtos;

namespace FoodioAPI.Services;

/// <summary>
/// Interface định nghĩa các phương thức business logic cho quản lý ca làm việc
/// Service layer xử lý validation, business rules và orchestration
/// </summary>
public interface IShiftService
{
    /// <summary>
    /// Lấy tất cả ca làm việc với phân trang
    /// </summary>
    /// <param name="page">Số trang (bắt đầu từ 1)</param>
    /// <param name="pageSize">Số lượng item trên mỗi trang</param>
    /// <returns>Danh sách ca làm việc với thông tin phân trang</returns>
    Task<PaginatedData<ShiftDto>> GetAllShiftsAsync(int page = 1, int pageSize = 10);

    /// <summary>
    /// Lấy thông tin chi tiết ca làm việc theo ID
    /// </summary>
    /// <param name="id">ID của ca làm việc</param>
    /// <returns>Thông tin chi tiết ca làm việc hoặc null nếu không tìm thấy</returns>
    Task<ShiftDto?> GetShiftByIdAsync(Guid id);

    /// <summary>
    /// Lấy danh sách ca làm việc của một nhân viên
    /// </summary>
    /// <param name="userId">ID của nhân viên</param>
    /// <returns>Danh sách ca làm việc của nhân viên</returns>
    Task<List<ShiftDto>> GetShiftsByUserIdAsync(string userId);

    /// <summary>
    /// Lấy danh sách ca làm việc trong khoảng thời gian
    /// </summary>
    /// <param name="startDate">Ngày bắt đầu</param>
    /// <param name="endDate">Ngày kết thúc</param>
    /// <returns>Danh sách ca làm việc trong khoảng thời gian</returns>
    Task<List<ShiftDto>> GetShiftsByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Lấy danh sách ca làm việc của một nhân viên trong khoảng thời gian
    /// </summary>
    /// <param name="userId">ID của nhân viên</param>
    /// <param name="startDate">Ngày bắt đầu</param>
    /// <param name="endDate">Ngày kết thúc</param>
    /// <returns>Danh sách ca làm việc</returns>
    Task<List<ShiftDto>> GetShiftsByUserAndDateRangeAsync(string userId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Lấy danh sách ca làm việc theo vai trò
    /// </summary>
    /// <param name="role">Vai trò ca làm việc</param>
    /// <returns>Danh sách ca làm việc theo vai trò</returns>
    Task<List<ShiftDto>> GetShiftsByRoleAsync(string role);

    /// <summary>
    /// Tạo ca làm việc mới
    /// Bao gồm validation và business rules
    /// </summary>
    /// <param name="createShiftDto">Thông tin ca làm việc cần tạo</param>
    /// <returns>Thông tin ca làm việc đã tạo</returns>
    Task<ShiftDto> CreateShiftAsync(CreateShiftDto createShiftDto);

    /// <summary>
    /// Cập nhật thông tin ca làm việc
    /// </summary>
    /// <param name="id">ID của ca làm việc cần cập nhật</param>
    /// <param name="updateShiftDto">Thông tin cập nhật</param>
    /// <returns>Thông tin ca làm việc sau khi cập nhật</returns>
    Task<ShiftDto> UpdateShiftAsync(Guid id, CreateShiftDto updateShiftDto);

    /// <summary>
    /// Xóa ca làm việc
    /// </summary>
    /// <param name="id">ID của ca làm việc cần xóa</param>
    /// <returns>True nếu xóa thành công, False nếu không</returns>
    Task<bool> DeleteShiftAsync(Guid id);

    /// <summary>
    /// Kiểm tra xem ca làm việc có bị trùng thời gian không
    /// </summary>
    /// <param name="userId">ID nhân viên</param>
    /// <param name="startTime">Thời gian bắt đầu</param>
    /// <param name="endTime">Thời gian kết thúc</param>
    /// <param name="excludeShiftId">ID ca làm việc cần loại trừ</param>
    /// <returns>True nếu có ca trùng, False nếu không</returns>
    Task<bool> HasOverlappingShiftAsync(string userId, DateTime startTime, DateTime endTime, Guid? excludeShiftId = null);

    /// <summary>
    /// Kiểm tra xem nhân viên có tồn tại không
    /// </summary>
    /// <param name="userId">ID của nhân viên</param>
    /// <returns>True nếu nhân viên tồn tại, False nếu không</returns>
    Task<bool> UserExistsAsync(string userId);
} 