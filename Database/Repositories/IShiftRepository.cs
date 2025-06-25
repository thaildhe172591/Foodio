using FoodioAPI.Entities;

namespace FoodioAPI.Database.Repositories;

/// <summary>
/// Interface định nghĩa các phương thức truy cập dữ liệu cho ca làm việc
/// Kế thừa từ IBaseRepository để có các phương thức cơ bản
/// </summary>
public interface IShiftRepository : IBaseRepository<Shift>
{
    /// <summary>
    /// Lấy danh sách ca làm việc của một nhân viên cụ thể
    /// </summary>
    /// <param name="userId">ID của nhân viên</param>
    /// <returns>Danh sách ca làm việc của nhân viên</returns>
    Task<List<Shift>> GetShiftsByUserIdAsync(string userId);

    /// <summary>
    /// Lấy danh sách ca làm việc trong khoảng thời gian
    /// </summary>
    /// <param name="startDate">Ngày bắt đầu</param>
    /// <param name="endDate">Ngày kết thúc</param>
    /// <returns>Danh sách ca làm việc trong khoảng thời gian</returns>
    Task<List<Shift>> GetShiftsByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Lấy danh sách ca làm việc của một nhân viên trong khoảng thời gian
    /// </summary>
    /// <param name="userId">ID của nhân viên</param>
    /// <param name="startDate">Ngày bắt đầu</param>
    /// <param name="endDate">Ngày kết thúc</param>
    /// <returns>Danh sách ca làm việc</returns>
    Task<List<Shift>> GetShiftsByUserAndDateRangeAsync(string userId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Kiểm tra xem có ca làm việc nào trùng thời gian không
    /// </summary>
    /// <param name="userId">ID nhân viên</param>
    /// <param name="startTime">Thời gian bắt đầu</param>
    /// <param name="endTime">Thời gian kết thúc</param>
    /// <param name="excludeShiftId">ID ca làm việc cần loại trừ (dùng khi update)</param>
    /// <returns>True nếu có ca trùng, False nếu không</returns>
    Task<bool> HasOverlappingShiftAsync(string userId, DateTime startTime, DateTime endTime, Guid? excludeShiftId = null);

    /// <summary>
    /// Lấy danh sách ca làm việc theo vai trò
    /// </summary>
    /// <param name="role">Vai trò ca làm việc</param>
    /// <returns>Danh sách ca làm việc theo vai trò</returns>
    Task<List<Shift>> GetShiftsByRoleAsync(string role);
} 