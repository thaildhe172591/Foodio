using FoodioAPI.DTOs;
using FoodioAPI.DTOs.UserDtos;
using FoodioAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodioAPI.Controllers;

/// <summary>
/// Controller quản lý ca làm việc
/// Cung cấp các API endpoint để CRUD ca làm việc của nhân viên
/// </summary>
[Route("api/admin/shifts")]
[ApiController]
//[Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền quản lý ca làm việc
public class ShiftController : ControllerBase
{
    private readonly IShiftService _shiftService;

    /// <summary>
    /// Constructor nhận IShiftService thông qua dependency injection
    /// </summary>
    /// <param name="shiftService">Service xử lý business logic ca làm việc</param>
    public ShiftController(IShiftService shiftService)
    {
        _shiftService = shiftService;
    }

    /// <summary>
    /// Lấy tất cả ca làm việc với phân trang
    /// </summary>
    /// <param name="page">Số trang (mặc định: 1)</param>
    /// <param name="pageSize">Số lượng item trên mỗi trang (mặc định: 10, tối đa: 100)</param>
    /// <returns>Danh sách ca làm việc với thông tin phân trang</returns>
    /// <response code="200">Trả về danh sách ca làm việc thành công</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền truy cập</response>
    [HttpGet]
    [ProducesResponseType(typeof(Response<PaginatedData<ShiftDto>>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<Response<PaginatedData<ShiftDto>>>> GetAllShifts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _shiftService.GetAllShiftsAsync(page, pageSize);
            return Ok(new Response<PaginatedData<ShiftDto>>
            {
                Status = ResponseStatus.SUCCESS,
                Message = "Lấy danh sách ca làm việc thành công",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new Response<PaginatedData<ShiftDto>>
            {
                Status = ResponseStatus.ERROR,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Lấy thông tin chi tiết ca làm việc theo ID
    /// </summary>
    /// <param name="id">ID của ca làm việc</param>
    /// <returns>Thông tin chi tiết ca làm việc</returns>
    /// <response code="200">Trả về thông tin ca làm việc thành công</response>
    /// <response code="404">Không tìm thấy ca làm việc</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền truy cập</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Response<ShiftDto>), 200)]
    [ProducesResponseType(typeof(Response<ShiftDto>), 404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<Response<ShiftDto>>> GetShiftById(Guid id)
    {
        try
        {
            var shift = await _shiftService.GetShiftByIdAsync(id);
            if (shift == null)
                return NotFound(new Response<ShiftDto>
                {
                    Status = ResponseStatus.ERROR,
                    Message = "Không tìm thấy ca làm việc"
                });

            return Ok(new Response<ShiftDto>
            {
                Status = ResponseStatus.SUCCESS,
                Message = "Lấy thông tin ca làm việc thành công",
                Data = shift
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new Response<ShiftDto>
            {
                Status = ResponseStatus.ERROR,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Lấy danh sách ca làm việc của một nhân viên cụ thể
    /// </summary>
    /// <param name="userId">ID của nhân viên</param>
    /// <returns>Danh sách ca làm việc của nhân viên</returns>
    /// <response code="200">Trả về danh sách ca làm việc thành công</response>
    /// <response code="404">Không tìm thấy nhân viên</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền truy cập</response>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(Response<List<ShiftDto>>), 200)]
    [ProducesResponseType(typeof(Response<List<ShiftDto>>), 404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<Response<List<ShiftDto>>>> GetShiftsByUserId(string userId)
    {
        try
        {
            var shifts = await _shiftService.GetShiftsByUserIdAsync(userId);
            return Ok(new Response<List<ShiftDto>>
            {
                Status = ResponseStatus.SUCCESS,
                Message = "Lấy danh sách ca làm việc của nhân viên thành công",
                Data = shifts
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new Response<List<ShiftDto>>
            {
                Status = ResponseStatus.ERROR,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Lấy danh sách ca làm việc trong khoảng thời gian
    /// </summary>
    /// <param name="startDate">Ngày bắt đầu (format: yyyy-MM-dd)</param>
    /// <param name="endDate">Ngày kết thúc (format: yyyy-MM-dd)</param>
    /// <returns>Danh sách ca làm việc trong khoảng thời gian</returns>
    /// <response code="200">Trả về danh sách ca làm việc thành công</response>
    /// <response code="400">Khoảng thời gian không hợp lệ</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền truy cập</response>
    [HttpGet("date-range")]
    [ProducesResponseType(typeof(Response<List<ShiftDto>>), 200)]
    [ProducesResponseType(typeof(Response<List<ShiftDto>>), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<Response<List<ShiftDto>>>> GetShiftsByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var shifts = await _shiftService.GetShiftsByDateRangeAsync(startDate, endDate);
            return Ok(new Response<List<ShiftDto>>
            {
                Status = ResponseStatus.SUCCESS,
                Message = "Lấy danh sách ca làm việc theo thời gian thành công",
                Data = shifts
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new Response<List<ShiftDto>>
            {
                Status = ResponseStatus.ERROR,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Lấy danh sách ca làm việc của nhân viên trong khoảng thời gian
    /// </summary>
    /// <param name="userId">ID của nhân viên</param>
    /// <param name="startDate">Ngày bắt đầu</param>
    /// <param name="endDate">Ngày kết thúc</param>
    /// <returns>Danh sách ca làm việc</returns>
    /// <response code="200">Trả về danh sách ca làm việc thành công</response>
    /// <response code="400">Tham số không hợp lệ</response>
    /// <response code="404">Không tìm thấy nhân viên</response>
    [HttpGet("user/{userId}/date-range")]
    [ProducesResponseType(typeof(Response<List<ShiftDto>>), 200)]
    [ProducesResponseType(typeof(Response<List<ShiftDto>>), 400)]
    [ProducesResponseType(typeof(Response<List<ShiftDto>>), 404)]
    public async Task<ActionResult<Response<List<ShiftDto>>>> GetShiftsByUserAndDateRange(
        string userId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var shifts = await _shiftService.GetShiftsByUserAndDateRangeAsync(userId, startDate, endDate);
            return Ok(new Response<List<ShiftDto>>
            {
                Status = ResponseStatus.SUCCESS,
                Message = "Lấy danh sách ca làm việc của nhân viên theo thời gian thành công",
                Data = shifts
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new Response<List<ShiftDto>>
            {
                Status = ResponseStatus.ERROR,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Lấy danh sách ca làm việc theo vai trò
    /// </summary>
    /// <param name="role">Vai trò (Admin, Cashier, Kitchen, Shipper)</param>
    /// <returns>Danh sách ca làm việc theo vai trò</returns>
    /// <response code="200">Trả về danh sách ca làm việc thành công</response>
    /// <response code="400">Vai trò không hợp lệ</response>
    [HttpGet("role/{role}")]
    [ProducesResponseType(typeof(Response<List<ShiftDto>>), 200)]
    [ProducesResponseType(typeof(Response<List<ShiftDto>>), 400)]
    public async Task<ActionResult<Response<List<ShiftDto>>>> GetShiftsByRole(string role)
    {
        try
        {
            var shifts = await _shiftService.GetShiftsByRoleAsync(role);
            return Ok(new Response<List<ShiftDto>>
            {
                Status = ResponseStatus.SUCCESS,
                Message = $"Lấy danh sách ca làm việc vai trò {role} thành công",
                Data = shifts
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new Response<List<ShiftDto>>
            {
                Status = ResponseStatus.ERROR,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Tạo ca làm việc mới
    /// </summary>
    /// <param name="createShiftDto">Thông tin ca làm việc cần tạo</param>
    /// <returns>Thông tin ca làm việc đã tạo</returns>
    /// <response code="201">Tạo ca làm việc thành công</response>
    /// <response code="400">Dữ liệu không hợp lệ hoặc vi phạm business rules</response>
    /// <response code="404">Không tìm thấy nhân viên</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền truy cập</response>
    [HttpPost]
    [ProducesResponseType(typeof(Response<ShiftDto>), 201)]
    [ProducesResponseType(typeof(Response<ShiftDto>), 400)]
    [ProducesResponseType(typeof(Response<ShiftDto>), 404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<Response<ShiftDto>>> CreateShift([FromBody] CreateShiftDto createShiftDto)
    {
        try
        {
            var result = await _shiftService.CreateShiftAsync(createShiftDto);
            return CreatedAtAction(nameof(GetShiftById), new { id = result.Id }, new Response<ShiftDto>
            {
                Status = ResponseStatus.SUCCESS,
                Message = "Tạo ca làm việc thành công",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new Response<ShiftDto>
            {
                Status = ResponseStatus.ERROR,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Cập nhật thông tin ca làm việc
    /// </summary>
    /// <param name="id">ID của ca làm việc cần cập nhật</param>
    /// <param name="updateShiftDto">Thông tin cập nhật</param>
    /// <returns>Thông tin ca làm việc sau khi cập nhật</returns>
    /// <response code="200">Cập nhật ca làm việc thành công</response>
    /// <response code="400">Dữ liệu không hợp lệ hoặc vi phạm business rules</response>
    /// <response code="404">Không tìm thấy ca làm việc hoặc nhân viên</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền truy cập</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Response<ShiftDto>), 200)]
    [ProducesResponseType(typeof(Response<ShiftDto>), 400)]
    [ProducesResponseType(typeof(Response<ShiftDto>), 404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<Response<ShiftDto>>> UpdateShift(Guid id, [FromBody] CreateShiftDto updateShiftDto)
    {
        try
        {
            var result = await _shiftService.UpdateShiftAsync(id, updateShiftDto);
            return Ok(new Response<ShiftDto>
            {
                Status = ResponseStatus.SUCCESS,
                Message = "Cập nhật ca làm việc thành công",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new Response<ShiftDto>
            {
                Status = ResponseStatus.ERROR,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Xóa ca làm việc
    /// </summary>
    /// <param name="id">ID của ca làm việc cần xóa</param>
    /// <returns>Kết quả xóa ca làm việc</returns>
    /// <response code="200">Xóa ca làm việc thành công</response>
    /// <response code="404">Không tìm thấy ca làm việc</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền truy cập</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Response<bool>), 200)]
    [ProducesResponseType(typeof(Response<bool>), 404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<Response<bool>>> DeleteShift(Guid id)
    {
        try
        {
            var result = await _shiftService.DeleteShiftAsync(id);
            return Ok(new Response<bool>
            {
                Status = ResponseStatus.SUCCESS,
                Message = "Xóa ca làm việc thành công",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new Response<bool>
            {
                Status = ResponseStatus.ERROR,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Kiểm tra xem ca làm việc có bị trùng thời gian không
    /// Endpoint hỗ trợ cho việc validation trước khi tạo/cập nhật ca làm việc
    /// </summary>
    /// <param name="userId">ID nhân viên</param>
    /// <param name="startTime">Thời gian bắt đầu</param>
    /// <param name="endTime">Thời gian kết thúc</param>
    /// <param name="excludeShiftId">ID ca làm việc cần loại trừ (optional, dùng khi update)</param>
    /// <returns>True nếu có ca trùng, False nếu không</returns>
    /// <response code="200">Kiểm tra thành công</response>
    /// <response code="400">Tham số không hợp lệ</response>
    [HttpGet("check-overlap")]
    [ProducesResponseType(typeof(Response<bool>), 200)]
    [ProducesResponseType(typeof(Response<bool>), 400)]
    public async Task<ActionResult<Response<bool>>> CheckOverlap(
        [FromQuery] string userId,
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime,
        [FromQuery] Guid? excludeShiftId = null)
    {
        try
        {
            var hasOverlap = await _shiftService.HasOverlappingShiftAsync(userId, startTime, endTime, excludeShiftId);
            return Ok(new Response<bool>
            {
                Status = ResponseStatus.SUCCESS,
                Message = hasOverlap ? "Có ca làm việc trùng thời gian" : "Không có ca làm việc trùng thời gian",
                Data = hasOverlap
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new Response<bool>
            {
                Status = ResponseStatus.ERROR,
                Message = ex.Message
            });
        }
    }
}