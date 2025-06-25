using FoodioAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodioAPI.Database.Repositories.Implements;

/// <summary>
/// Repository implementation cho ca làm việc
/// Xử lý tất cả các thao tác truy cập dữ liệu liên quan đến ca làm việc
/// </summary>
public class ShiftRepository : BaseRepository<Shift>, IShiftRepository
{
    /// <summary>
    /// Constructor nhận ApplicationDbContext thông qua dependency injection
    /// </summary>
    /// <param name="context">Database context để truy cập dữ liệu</param>
    public ShiftRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Lấy danh sách ca làm việc của một nhân viên cụ thể
    /// Bao gồm thông tin User để hiển thị
    /// </summary>
    /// <param name="userId">ID của nhân viên</param>
    /// <returns>Danh sách ca làm việc được sắp xếp theo thời gian</returns>
    public async Task<List<Shift>> GetShiftsByUserIdAsync(string userId)
    {
        return await Entities
            .Include(s => s.User) // Eager loading User info
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.StartTime) // Sắp xếp ca mới nhất trước
            .ToListAsync();
    }

    /// <summary>
    /// Lấy danh sách ca làm việc trong khoảng thời gian
    /// Hữu ích cho báo cáo và thống kê
    /// </summary>
    /// <param name="startDate">Ngày bắt đầu (inclusive)</param>
    /// <param name="endDate">Ngày kết thúc (inclusive)</param>
    /// <returns>Danh sách ca làm việc trong khoảng thời gian</returns>
    public async Task<List<Shift>> GetShiftsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        // Chuyển đổi sang UTC để đảm bảo tính nhất quán
        var startUtc = startDate.ToUniversalTime();
        var endUtc = endDate.ToUniversalTime().AddDays(1).AddTicks(-1); // Cuối ngày

        return await Entities
            .Include(s => s.User)
            .Where(s => s.StartTime >= startUtc && s.StartTime <= endUtc)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy danh sách ca làm việc của một nhân viên trong khoảng thời gian
    /// Kết hợp filter theo user và thời gian
    /// </summary>
    /// <param name="userId">ID của nhân viên</param>
    /// <param name="startDate">Ngày bắt đầu</param>
    /// <param name="endDate">Ngày kết thúc</param>
    /// <returns>Danh sách ca làm việc</returns>
    public async Task<List<Shift>> GetShiftsByUserAndDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
    {
        var startUtc = startDate.ToUniversalTime();
        var endUtc = endDate.ToUniversalTime().AddDays(1).AddTicks(-1);

        return await Entities
            .Include(s => s.User)
            .Where(s => s.UserId == userId &&
                       s.StartTime >= startUtc &&
                       s.StartTime <= endUtc)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    /// <summary>
    /// Kiểm tra xem có ca làm việc nào trùng thời gian không
    /// Đảm bảo một nhân viên không thể có 2 ca trùng thời gian
    /// </summary>
    /// <param name="userId">ID nhân viên</param>
    /// <param name="startTime">Thời gian bắt đầu ca mới</param>
    /// <param name="endTime">Thời gian kết thúc ca mới</param>
    /// <param name="excludeShiftId">ID ca làm việc cần loại trừ (dùng khi update)</param>
    /// <returns>True nếu có ca trùng, False nếu không</returns>
    public async Task<bool> HasOverlappingShiftAsync(string userId, DateTime startTime, DateTime endTime, Guid? excludeShiftId = null)
    {
        var query = Entities.Where(s => s.UserId == userId);

        // Loại trừ ca làm việc hiện tại khi update
        if (excludeShiftId.HasValue)
        {
            query = query.Where(s => s.Id != excludeShiftId.Value);
        }

        // Kiểm tra trùng thời gian:
        // Ca mới bắt đầu trước khi ca cũ kết thúc VÀ ca mới kết thúc sau khi ca cũ bắt đầu
        var hasOverlap = await query.AnyAsync(s =>
            startTime < s.EndTime && endTime > s.StartTime);

        return hasOverlap;
    }

    /// <summary>
    /// Lấy danh sách ca làm việc theo vai trò
    /// Hữu ích cho việc phân công và quản lý theo department
    /// </summary>
    /// <param name="role">Vai trò ca làm việc (Admin, Cashier, Kitchen, Shipper)</param>
    /// <returns>Danh sách ca làm việc theo vai trò</returns>
    public async Task<List<Shift>> GetShiftsByRoleAsync(string role)
    {
        return await Entities
            .Include(s => s.User)
            .Where(s => s.Role == role)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    /// <summary>
    /// Ẩn phương thức GetByIdAsync của base class để bao gồm User information
    /// </summary>
    /// <param name="id">ID của ca làm việc</param>
    /// <returns>Ca làm việc với thông tin User</returns>
    public new async Task<Shift?> GetByIdAsync(Guid id)
    {
        return await Entities
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    /// <summary>
    /// Ẩn phương thức GetAllAsync của base class để bao gồm User information
    /// </summary>
    /// <returns>Tất cả ca làm việc với thông tin User</returns>
    public new async Task<List<Shift>> GetAllAsync()
    {
        return await Entities
            .Include(s => s.User)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();
    }
}