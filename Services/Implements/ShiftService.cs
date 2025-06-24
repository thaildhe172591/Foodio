using FoodioAPI.Constants;
using FoodioAPI.Database.Repositories;
using FoodioAPI.DTOs;
using FoodioAPI.DTOs.UserDtos;
using FoodioAPI.Entities;
using FoodioAPI.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace FoodioAPI.Services.Implements;

/// <summary>
/// Service implementation cho quản lý ca làm việc
/// Xử lý business logic, validation và orchestration cho các thao tác ca làm việc
/// </summary>
public class ShiftService : IShiftService
{
    private readonly IShiftRepository _shiftRepository;
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// Constructor nhận dependencies thông qua dependency injection
    /// </summary>
    /// <param name="shiftRepository">Repository để truy cập dữ liệu ca làm việc</param>
    /// <param name="userManager">User manager để kiểm tra thông tin nhân viên</param>
    public ShiftService(IShiftRepository shiftRepository, UserManager<User> userManager)
    {
        _shiftRepository = shiftRepository;
        _userManager = userManager;
    }

    /// <summary>
    /// Lấy tất cả ca làm việc với phân trang
    /// </summary>
    /// <param name="page">Số trang (bắt đầu từ 1)</param>
    /// <param name="pageSize">Số lượng item trên mỗi trang</param>
    /// <returns>Danh sách ca làm việc với thông tin phân trang</returns>
    public async Task<PaginatedData<ShiftDto>> GetAllShiftsAsync(int page = 1, int pageSize = 10)
    {
        // Validation input parameters
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var allShifts = await _shiftRepository.GetAllAsync();
        var totalCount = allShifts.Count;

        // Áp dụng phân trang
        var pagedShifts = allShifts
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Chuyển đổi sang DTO
        var shiftDtos = pagedShifts.Select(MapToDto).ToList();

        return new PaginatedData<ShiftDto>(shiftDtos, totalCount, page, pageSize);
    }

    /// <summary>
    /// Lấy thông tin chi tiết ca làm việc theo ID
    /// </summary>
    /// <param name="id">ID của ca làm việc</param>
    /// <returns>Thông tin chi tiết ca làm việc hoặc null nếu không tìm thấy</returns>
    public async Task<ShiftDto?> GetShiftByIdAsync(Guid id)
    {
        var shift = await _shiftRepository.GetByIdAsync(id);
        return shift != null ? MapToDto(shift) : null;
    }

    /// <summary>
    /// Lấy danh sách ca làm việc của một nhân viên
    /// </summary>
    /// <param name="userId">ID của nhân viên</param>
    /// <returns>Danh sách ca làm việc của nhân viên</returns>
    /// <exception cref="NotFoundException">Khi không tìm thấy nhân viên</exception>
    public async Task<List<ShiftDto>> GetShiftsByUserIdAsync(string userId)
    {
        // Kiểm tra nhân viên có tồn tại không
        if (!await UserExistsAsync(userId))
            throw new NotFoundException($"Nhân viên với ID {userId} không tồn tại");

        var shifts = await _shiftRepository.GetShiftsByUserIdAsync(userId);
        return shifts.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Lấy danh sách ca làm việc trong khoảng thời gian
    /// </summary>
    /// <param name="startDate">Ngày bắt đầu</param>
    /// <param name="endDate">Ngày kết thúc</param>
    /// <returns>Danh sách ca làm việc trong khoảng thời gian</returns>
    /// <exception cref="BadRequestException">Khi khoảng thời gian không hợp lệ</exception>
    public async Task<List<ShiftDto>> GetShiftsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        // Validation thời gian
        if (endDate < startDate)
            throw new BadRequestException("Ngày kết thúc phải sau hoặc bằng ngày bắt đầu");

        var shifts = await _shiftRepository.GetShiftsByDateRangeAsync(startDate, endDate);
        return shifts.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Lấy danh sách ca làm việc của một nhân viên trong khoảng thời gian
    /// </summary>
    /// <param name="userId">ID của nhân viên</param>
    /// <param name="startDate">Ngày bắt đầu</param>
    /// <param name="endDate">Ngày kết thúc</param>
    /// <returns>Danh sách ca làm việc</returns>
    public async Task<List<ShiftDto>> GetShiftsByUserAndDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
    {
        // Validation
        if (!await UserExistsAsync(userId))
            throw new NotFoundException($"Nhân viên với ID {userId} không tồn tại");

        if (endDate < startDate)
            throw new BadRequestException("Ngày kết thúc phải sau hoặc bằng ngày bắt đầu");

        var shifts = await _shiftRepository.GetShiftsByUserAndDateRangeAsync(userId, startDate, endDate);
        return shifts.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Lấy danh sách ca làm việc theo vai trò
    /// </summary>
    /// <param name="role">Vai trò ca làm việc</param>
    /// <returns>Danh sách ca làm việc theo vai trò</returns>
    /// <exception cref="BadRequestException">Khi vai trò không hợp lệ</exception>
    public async Task<List<ShiftDto>> GetShiftsByRoleAsync(string role)
    {
        // Validation vai trò hợp lệ
        if (!IsValidRole(role))
            throw new BadRequestException($"Vai trò '{role}' không hợp lệ");

        var shifts = await _shiftRepository.GetShiftsByRoleAsync(role);
        return shifts.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Tạo ca làm việc mới
    /// Bao gồm validation và business rules
    /// </summary>
    /// <param name="createShiftDto">Thông tin ca làm việc cần tạo</param>
    /// <returns>Thông tin ca làm việc đã tạo</returns>
    /// <exception cref="ValidationException">Khi dữ liệu không hợp lệ</exception>
    /// <exception cref="BadRequestException">Khi vi phạm business rules</exception>
    public async Task<ShiftDto> CreateShiftAsync(CreateShiftDto createShiftDto)
    {
        // Validation cơ bản
        await ValidateShiftDataAsync(createShiftDto);

        // Kiểm tra trùng ca làm việc
        if (await HasOverlappingShiftAsync(createShiftDto.UserId, createShiftDto.StartTime, createShiftDto.EndTime))
            throw new BadRequestException("Nhân viên đã có ca làm việc trùng thời gian");

        // Tạo entity từ DTO
        var shift = new Shift
        {
            Id = Guid.NewGuid(),
            UserId = createShiftDto.UserId,
            StartTime = createShiftDto.StartTime.ToUniversalTime(),
            EndTime = createShiftDto.EndTime.ToUniversalTime(),
            Role = createShiftDto.Role
        };

        // Lưu vào database
        var createdShift = await _shiftRepository.AddAsync(shift);
        await _shiftRepository.SaveChangeAsync();

        // Lấy thông tin đầy đủ với User data
        var shiftWithUser = await _shiftRepository.GetByIdAsync(createdShift.Id);
        return MapToDto(shiftWithUser!);
    }

    /// <summary>
    /// Cập nhật thông tin ca làm việc
    /// </summary>
    /// <param name="id">ID của ca làm việc cần cập nhật</param>
    /// <param name="updateShiftDto">Thông tin cập nhật</param>
    /// <returns>Thông tin ca làm việc sau khi cập nhật</returns>
    /// <exception cref="NotFoundException">Khi không tìm thấy ca làm việc</exception>
    /// <exception cref="BadRequestException">Khi vi phạm business rules</exception>
    public async Task<ShiftDto> UpdateShiftAsync(Guid id, CreateShiftDto updateShiftDto)
    {
        // Kiểm tra ca làm việc có tồn tại
        var existingShift = await _shiftRepository.GetByIdAsync(id);
        if (existingShift == null)
            throw new NotFoundException($"Ca làm việc với ID {id} không tồn tại");

        // Validation dữ liệu mới
        await ValidateShiftDataAsync(updateShiftDto);

        // Kiểm tra trùng ca làm việc (loại trừ ca hiện tại)
        if (await HasOverlappingShiftAsync(updateShiftDto.UserId, updateShiftDto.StartTime, updateShiftDto.EndTime, id))
            throw new BadRequestException("Nhân viên đã có ca làm việc trùng thời gian");

        // Cập nhật thông tin
        existingShift.UserId = updateShiftDto.UserId;
        existingShift.StartTime = updateShiftDto.StartTime.ToUniversalTime();
        existingShift.EndTime = updateShiftDto.EndTime.ToUniversalTime();
        existingShift.Role = updateShiftDto.Role;

        await _shiftRepository.UpdateAsync(existingShift);
        await _shiftRepository.SaveChangeAsync();

        // Lấy thông tin đầy đủ sau khi cập nhật
        var updatedShift = await _shiftRepository.GetByIdAsync(id);
        return MapToDto(updatedShift!);
    }

    /// <summary>
    /// Xóa ca làm việc
    /// </summary>
    /// <param name="id">ID của ca làm việc cần xóa</param>
    /// <returns>True nếu xóa thành công, False nếu không</returns>
    /// <exception cref="NotFoundException">Khi không tìm thấy ca làm việc</exception>
    public async Task<bool> DeleteShiftAsync(Guid id)
    {
        var shift = await _shiftRepository.GetByIdAsync(id);
        if (shift == null)
            throw new NotFoundException($"Ca làm việc với ID {id} không tồn tại");

        await _shiftRepository.DeleteAsync(shift);
        await _shiftRepository.SaveChangeAsync();
        return true;
    }

    /// <summary>
    /// Kiểm tra xem ca làm việc có bị trùng thời gian không
    /// </summary>
    /// <param name="userId">ID nhân viên</param>
    /// <param name="startTime">Thời gian bắt đầu</param>
    /// <param name="endTime">Thời gian kết thúc</param>
    /// <param name="excludeShiftId">ID ca làm việc cần loại trừ</param>
    /// <returns>True nếu có ca trùng, False nếu không</returns>
    public async Task<bool> HasOverlappingShiftAsync(string userId, DateTime startTime, DateTime endTime, Guid? excludeShiftId = null)
    {
        return await _shiftRepository.HasOverlappingShiftAsync(userId, startTime, endTime, excludeShiftId);
    }

    /// <summary>
    /// Kiểm tra xem nhân viên có tồn tại không
    /// </summary>
    /// <param name="userId">ID của nhân viên</param>
    /// <returns>True nếu nhân viên tồn tại, False nếu không</returns>
    public async Task<bool> UserExistsAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null;
    }

    #region Private Helper Methods

    /// <summary>
    /// Validation dữ liệu ca làm việc
    /// </summary>
    /// <param name="shiftDto">Dữ liệu ca làm việc cần validate</param>
    private async Task ValidateShiftDataAsync(CreateShiftDto shiftDto)
    {
        // Kiểm tra nhân viên có tồn tại
        if (!await UserExistsAsync(shiftDto.UserId))
            throw new NotFoundException($"Nhân viên với ID {shiftDto.UserId} không tồn tại");

        // Kiểm tra thời gian hợp lệ
        if (shiftDto.EndTime <= shiftDto.StartTime)
            throw new BadRequestException("Thời gian kết thúc phải sau thời gian bắt đầu");

        // Kiểm tra ca làm việc không quá 12 tiếng
        var duration = shiftDto.EndTime - shiftDto.StartTime;
        if (duration.TotalHours > 12)
            throw new BadRequestException("Ca làm việc không được vượt quá 12 tiếng");

        // Kiểm tra ca làm việc tối thiểu 1 tiếng
        if (duration.TotalMinutes < 60)
            throw new BadRequestException("Ca làm việc phải ít nhất 1 tiếng");

        // Kiểm tra vai trò hợp lệ
        if (!IsValidRole(shiftDto.Role))
            throw new BadRequestException($"Vai trò '{shiftDto.Role}' không hợp lệ");

        // Kiểm tra không tạo ca làm việc trong quá khứ (trừ hôm nay)
        //if (shiftDto.StartTime.Date < DateTime.Today)
        //    throw new BadRequestException("Không thể tạo ca làm việc trong quá khứ");
    }

    /// <summary>
    /// Kiểm tra vai trò có hợp lệ không
    /// </summary>
    /// <param name="role">Vai trò cần kiểm tra</param>
    /// <returns>True nếu hợp lệ, False nếu không</returns>
    private static bool IsValidRole(string role)
    {
        var validRoles = new[] { UserRole.Admin, UserRole.Cashier, UserRole.Kitchen, UserRole.Shipper };
        return validRoles.Contains(role);
    }

    /// <summary>
    /// Chuyển đổi Shift entity sang ShiftDto
    /// </summary>
    /// <param name="shift">Shift entity</param>
    /// <returns>ShiftDto</returns>
    private static ShiftDto MapToDto(Shift shift)
    {
        return new ShiftDto
        {
            Id = shift.Id,
            UserId = shift.UserId,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            Role = shift.Role
        };
    }

    #endregion
}