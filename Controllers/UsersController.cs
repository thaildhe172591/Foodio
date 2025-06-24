using FoodioAPI.DTOs;
using FoodioAPI.DTOs.UserDtos;
using FoodioAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodioAPI.Controllers
{
    /// <summary>
    /// Controller quản lý người dùng cho Admin
    /// Cung cấp các API để quản lý danh sách người dùng, phân quyền, khóa/mở khóa tài khoản
    /// </summary>
    [Route("api/admin/users")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;

        public UsersController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng trong hệ thống với phân trang và tìm kiếm
        /// </summary>
        /// <param name="page">Số trang (mặc định: 1)</param>
        /// <param name="pageSize">Số lượng item trên mỗi trang (mặc định: 10)</param>
        /// <param name="searchTerm">Từ khóa tìm kiếm theo tên người dùng hoặc email</param>
        /// <returns>Danh sách người dùng với thông tin phân trang</returns>
        /// <response code="200">Trả về danh sách người dùng thành công</response>
        /// <response code="400">Lỗi khi xử lý yêu cầu</response>
        [HttpGet]
        [ProducesResponseType(typeof(Response<PaginatedData<UserDto>>), 200)]
        [ProducesResponseType(typeof(Response<PaginatedData<UserDto>>), 400)]
        public async Task<ActionResult<Response<PaginatedData<UserDto>>>> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                var result = await _userManagementService.GetUsersAsync(page, pageSize, searchTerm);
                return Ok(new Response<PaginatedData<UserDto>>
                {
                    Status = ResponseStatus.SUCCESS,
                    Message = "Lấy danh sách người dùng thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<PaginatedData<UserDto>>
                {
                    Status = ResponseStatus.ERROR,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy danh sách người dùng với các bộ lọc chi tiết
        /// Hỗ trợ tìm kiếm theo email, username, vai trò, trạng thái khóa, xác nhận email, v.v.
        /// </summary>
        /// <param name="searchDto">Các tham số tìm kiếm và lọc chi tiết</param>
        /// <returns>Danh sách người dùng với thông tin phân trang</returns>
        /// <response code="200">Trả về danh sách người dùng thành công</response>
        /// <response code="400">Lỗi khi xử lý yêu cầu</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(Response<PaginatedData<UserDto>>), 200)]
        [ProducesResponseType(typeof(Response<PaginatedData<UserDto>>), 400)]
        public async Task<ActionResult<Response<PaginatedData<UserDto>>>> SearchWithStaff(
            [FromQuery] UserSearchDto searchDto)
        {
            try
            {
                var result = await _userManagementService.SearchWithStaff(searchDto);
                return Ok(new Response<PaginatedData<UserDto>>
                {
                    Status = ResponseStatus.SUCCESS,
                    Message = "Lấy danh sách người dùng thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<PaginatedData<UserDto>>
                {
                    Status = ResponseStatus.ERROR,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một người dùng theo ID
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <returns>Thông tin chi tiết người dùng bao gồm vai trò và trạng thái</returns>
        /// <response code="200">Trả về thông tin người dùng thành công</response>
        /// <response code="404">Không tìm thấy người dùng</response>
        /// <response code="400">Lỗi khi xử lý yêu cầu</response>
        [HttpGet("get-by-id/{id}")]
        [ProducesResponseType(typeof(Response<UserDto>), 200)]
        [ProducesResponseType(typeof(Response<UserDto>), 404)]
        [ProducesResponseType(typeof(Response<UserDto>), 400)]
        public async Task<ActionResult<Response<UserDto>>> GetUser(string id)
        {
            try
            {
                var user = await _userManagementService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new Response<UserDto>
                    {
                        Status = ResponseStatus.ERROR,
                        Message = "Không tìm thấy người dùng"
                    });

                return Ok(new Response<UserDto>
                {
                    Status = ResponseStatus.SUCCESS,
                    Message = "Lấy thông tin người dùng thành công",
                    Data = user
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<UserDto>
                {
                    Status = ResponseStatus.ERROR,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một người dùng theo email
        /// </summary>
        /// <param name="email">Email của người dùng</param>
        /// <returns>Thông tin chi tiết người dùng bao gồm vai trò và trạng thái</returns>
        /// <response code="200">Trả về thông tin người dùng thành công</response>
        /// <response code="404">Không tìm thấy người dùng</response>
        /// <response code="400">Lỗi khi xử lý yêu cầu</response>
        [HttpGet("get-by-email/{email}")]
        [ProducesResponseType(typeof(Response<UserDto>), 200)]
        [ProducesResponseType(typeof(Response<UserDto>), 404)]
        [ProducesResponseType(typeof(Response<UserDto>), 400)]
        public async Task<ActionResult<Response<UserDto>>> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userManagementService.GetUserByEmailAsync(email);
                if (user == null)
                    return NotFound(new Response<UserDto>
                    {
                        Status = ResponseStatus.ERROR,
                        Message = "Không tìm thấy người dùng"
                    });

                return Ok(new Response<UserDto>
                {
                    Status = ResponseStatus.SUCCESS,
                    Message = "Lấy thông tin người dùng thành công",
                    Data = user
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<UserDto>
                {
                    Status = ResponseStatus.ERROR,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Cập nhật vai trò của người dùng
        /// Cho phép thay đổi quyền hạn của người dùng trong hệ thống
        /// Sử dụng Role IDs để bảo mật hơn - không truyền trực tiếp role names
        /// </summary>
        /// <param name="id">ID của người dùng cần cập nhật</param>
        /// <param name="dto">Danh sách Role IDs mới</param>
        /// <returns>Kết quả cập nhật vai trò</returns>
        /// <response code="200">Cập nhật vai trò thành công</response>
        /// <response code="400">Lỗi khi xử lý yêu cầu</response>
        [HttpPut("update-roles/{id}")]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        [ProducesResponseType(typeof(Response<bool>), 400)]
        public async Task<ActionResult<Response<bool>>> UpdateUserRole(string id, [FromBody] UpdateUserRoleDto dto)
        {
            try
            {
                var result = await _userManagementService.UpdateUserRoleAsync(id, dto.RoleIds);
                return Ok(new Response<bool>
                {
                    Status = ResponseStatus.SUCCESS,
                    Message = "Cập nhật vai trò người dùng thành công",
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
        /// Xóa người dùng khỏi hệ thống
        /// Lưu ý: Không thể xóa tài khoản system hoặc admin
        /// </summary>
        /// <param name="id">ID của người dùng cần xóa</param>
        /// <returns>Kết quả xóa người dùng</returns>
        /// <response code="200">Xóa người dùng thành công</response>
        /// <response code="400">Lỗi khi xử lý yêu cầu</response>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        [ProducesResponseType(typeof(Response<bool>), 400)]
        public async Task<ActionResult<Response<bool>>> DeleteUser(string id)
        {
            try
            {
                var result = await _userManagementService.DeleteUserAsync(id);
                return Ok(new Response<bool>
                {
                    Status = ResponseStatus.SUCCESS,
                    Message = "Xóa người dùng thành công",
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
        /// Khóa tài khoản người dùng trong một khoảng thời gian nhất định
        /// Người dùng sẽ không thể đăng nhập cho đến khi hết thời gian khóa
        /// </summary>
        /// <param name="id">ID của người dùng cần khóa</param>
        /// <param name="dto">Thông tin thời gian khóa (số ngày)</param>
        /// <returns>Kết quả khóa người dùng</returns>
        /// <response code="200">Khóa người dùng thành công</response>
        /// <response code="400">Lỗi khi xử lý yêu cầu</response>
        [HttpPost("lock/{id}")]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        [ProducesResponseType(typeof(Response<bool>), 400)]
        public async Task<ActionResult<Response<bool>>> LockUser(string id, [FromBody] LockUserDto dto)
        {
            try
            {
                var result = await _userManagementService.LockUserAsync(id, dto.LockoutDays);
                return Ok(new Response<bool>
                {
                    Status = ResponseStatus.SUCCESS,
                    Message = "Khóa người dùng thành công",
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
        /// Mở khóa tài khoản người dùng ngay lập tức
        /// Cho phép người dùng đăng nhập lại vào hệ thống
        /// </summary>
        /// <param name="id">ID của người dùng cần mở khóa</param>
        /// <returns>Kết quả mở khóa người dùng</returns>
        /// <response code="200">Mở khóa người dùng thành công</response>
        /// <response code="400">Lỗi khi xử lý yêu cầu</response>
        [HttpPost("unlock/{id}")]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        [ProducesResponseType(typeof(Response<bool>), 400)]
        public async Task<ActionResult<Response<bool>>> UnlockUser(string id)
        {
            try
            {
                var result = await _userManagementService.UnlockUserAsync(id);
                return Ok(new Response<bool>
                {
                    Status = ResponseStatus.SUCCESS,
                    Message = "Mở khóa người dùng thành công",
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
        /// Kiểm tra trạng thái khóa của người dùng
        /// Trả về true nếu tài khoản đang bị khóa, false nếu không
        /// </summary>
        /// <param name="id">ID của người dùng cần kiểm tra</param>
        /// <returns>Trạng thái khóa của người dùng</returns>
        /// <response code="200">Kiểm tra trạng thái khóa thành công</response>
        /// <response code="400">Lỗi khi xử lý yêu cầu</response>
        [HttpGet("check-lock-status/{id}")]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        [ProducesResponseType(typeof(Response<bool>), 400)]
        public async Task<ActionResult<Response<bool>>> IsUserLocked(string id)
        {
            try
            {
                var result = await _userManagementService.IsUserLockedAsync(id);
                return Ok(new Response<bool>
                {
                    Status = ResponseStatus.SUCCESS,
                    Message = "Kiểm tra trạng thái khóa thành công",
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
        /// Lấy thời gian mở khóa của người dùng
        /// Trả về null nếu tài khoản không bị khóa
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <returns>Thời gian mở khóa hoặc null</returns>
        /// <response code="200">Lấy thời gian mở khóa thành công</response>
        /// <response code="400">Lỗi khi xử lý yêu cầu</response>
        [HttpGet("get-lockout-end/{id}")]
        [ProducesResponseType(typeof(Response<DateTimeOffset?>), 200)]
        [ProducesResponseType(typeof(Response<DateTimeOffset?>), 400)]
        public async Task<ActionResult<Response<DateTimeOffset?>>> GetUserLockoutEndDate(string id)
        {
            try
            {
                var result = await _userManagementService.GetUserLockoutEndDateAsync(id);
                return Ok(new Response<DateTimeOffset?>
                {
                    Status = ResponseStatus.SUCCESS,
                    Message = "Lấy thời gian mở khóa thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<DateTimeOffset?>
                {
                    Status = ResponseStatus.ERROR,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả vai trò của một người dùng
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <returns>Danh sách tên vai trò</returns>
        /// <response code="200">Lấy danh sách vai trò thành công</response>
        /// <response code="400">Lỗi khi xử lý yêu cầu</response>
        [HttpGet("get-user-roles/{id}")]
        [ProducesResponseType(typeof(Response<List<string>>), 200)]
        [ProducesResponseType(typeof(Response<List<string>>), 400)]
        public async Task<ActionResult<Response<List<string>>>> GetUserRoles(string id)
        {
            try
            {
                var result = await _userManagementService.GetUserRolesAsync(id);
                return Ok(new Response<List<string>>
                {
                    Status = ResponseStatus.SUCCESS,
                    Message = "Lấy danh sách vai trò thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<List<string>>
                {
                    Status = ResponseStatus.ERROR,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả vai trò có trong hệ thống
        /// Dùng để hiển thị dropdown chọn vai trò khi cập nhật quyền người dùng
        /// Chỉ trả về thông tin an toàn, không expose sensitive data
        /// </summary>
        /// <returns>Danh sách vai trò với thông tin an toàn</returns>
        /// <response code="200">Lấy danh sách vai trò thành công</response>
        /// <response code="400">Lỗi khi xử lý yêu cầu</response>
        [HttpGet("get-all-roles")]
        [ProducesResponseType(typeof(Response<List<RoleDto>>), 200)]
        [ProducesResponseType(typeof(Response<List<RoleDto>>), 400)]
        public async Task<ActionResult<Response<List<RoleDto>>>> GetAllRoles()
        {
            try
            {
                var result = await _userManagementService.GetAllRolesAsync();
                return Ok(new Response<List<RoleDto>>
                {
                    Status = ResponseStatus.SUCCESS,
                    Message = "Lấy danh sách vai trò thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<List<RoleDto>>
                {
                    Status = ResponseStatus.ERROR,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Tạo người dùng mới
        /// Cho phép admin tạo tài khoản người dùng với vai trò cụ thể
        /// </summary>
        /// <param name="dto">Thông tin người dùng cần tạo</param>
        /// <returns>Thông tin người dùng đã tạo</returns>
        /// <response code="201">Tạo người dùng thành công</response>
        /// <response code="400">Lỗi khi xử lý yêu cầu</response>
        [HttpPost]
        [ProducesResponseType(typeof(Response<UserDto>), 201)]
        [ProducesResponseType(typeof(Response<UserDto>), 400)]
        public async Task<ActionResult<Response<UserDto>>> CreateUser([FromBody] CreateUserDto dto)
        {
            try
            {
                var result = await _userManagementService.CreateUserAsync(dto);
                return CreatedAtAction(nameof(GetUser), new { id = result.Id }, new Response<UserDto>
                {
                    Status = ResponseStatus.SUCCESS,
                    Message = "Tạo người dùng thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<UserDto>
                {
                    Status = ResponseStatus.ERROR,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Cập nhật thông tin cơ bản của người dùng (email, phone ...)
        /// </summary>
        /// <param name="id">ID người dùng</param>
        /// <param name="dto">Thông tin cập nhật</param>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        [ProducesResponseType(typeof(Response<bool>), 400)]
        public async Task<ActionResult<Response<bool>>> UpdateUser(string id, [FromBody] UpdateUserDto dto)
        {
            try
            {
                var result = await _userManagementService.UpdateUserAsync(id, dto);
                return Ok(new Response<bool>
                {
                    Status = ResponseStatus.SUCCESS,
                    Message = "Cập nhật thông tin người dùng thành công",
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
    }
}
