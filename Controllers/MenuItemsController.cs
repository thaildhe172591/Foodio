using FoodioAPI.DTOs.Menu;
using FoodioAPI.DTOs.UserDtos;
using FoodioAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodioAPI.Controllers
{
    /// <summary>
    /// Controller quản lý món ăn trong menu
    /// Cung cấp các API CRUD và quản lý trạng thái món ăn
    /// </summary>
    [Route("api/admin/menu-items")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class MenuItemsController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuItemsController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// - Lấy tất cả món ăn
        /// - Lọc theo trạng thái còn món/hết món
        /// - Lọc theo danh mục
        /// - Tìm kiếm theo tên
        /// - Lọc theo khoảng giá
        /// - Sắp xếp theo nhiều tiêu chí
        /// - Phân trang
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm theo tên món ăn</param>
        /// <param name="categoryId">ID danh mục để lọc</param>
        /// <param name="isAvailable">Trạng thái còn món/hết món (true: còn món, false: hết món)</param>
        /// <param name="minPrice">Giá tối thiểu</param>
        /// <param name="maxPrice">Giá tối đa</param>
        /// <param name="sortBy">Sắp xếp theo trường (name, price, createdDate)</param>
        /// <param name="sortOrder">Thứ tự sắp xếp (asc, desc)</param>
        /// <param name="page">Số trang (mặc định: 1)</param>
        /// <param name="pageSize">Số lượng món ăn trên trang (mặc định: 20, tối đa: 100)</param>
        /// <returns>Danh sách món ăn với thông tin phân trang và thống kê</returns>
        /// <response code="200">Trả về danh sách món ăn thành công</response>
        /// <response code="400">Tham số tìm kiếm không hợp lệ</response>
        /// <response code="500">Lỗi server khi lấy danh sách món ăn</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MenuItemSearchResultDto>> GetMenuItems(
            [FromQuery] string? searchTerm = null,
            [FromQuery] Guid? categoryId = null,
            [FromQuery] bool? isAvailable = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? sortBy = "name",
            [FromQuery] string? sortOrder = "asc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                // Validation cơ bản
                if (page < 1)
                {
                    return BadRequest(new { message = "Số trang phải lớn hơn 0" });
                }

                if (pageSize < 1 || pageSize > 100)
                {
                    return BadRequest(new { message = "Số lượng món ăn trên trang phải từ 1-100" });
                }

                if (minPrice.HasValue && maxPrice.HasValue && minPrice.Value > maxPrice.Value)
                {
                    return BadRequest(new { message = "Giá tối thiểu không được lớn hơn giá tối đa" });
                }

                if (minPrice.HasValue && minPrice.Value < 0)
                {
                    return BadRequest(new { message = "Giá tối thiểu không được âm" });
                }

                if (maxPrice.HasValue && maxPrice.Value < 0)
                {
                    return BadRequest(new { message = "Giá tối đa không được âm" });
                }

                // Tạo DTO tìm kiếm từ các tham số
                var searchDto = new MenuItemSearchDto
                {
                    SearchTerm = searchTerm?.Trim(),
                    CategoryId = categoryId,
                    IsAvailable = isAvailable,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    SortBy = sortBy?.ToLower(),
                    SortOrder = sortOrder?.ToLower(),
                    Page = page,
                    PageSize = pageSize
                };

                // Validation cho SortBy
                var validSortFields = new[] { "name", "price", "createddate" };
                if (!string.IsNullOrEmpty(searchDto.SortBy) && !validSortFields.Contains(searchDto.SortBy))
                {
                    return BadRequest(new { message = "Trường sắp xếp không hợp lệ. Các giá trị hợp lệ: name, price, createdDate" });
                }

                // Validation cho SortOrder
                var validSortOrders = new[] { "asc", "desc" };
                if (!string.IsNullOrEmpty(searchDto.SortOrder) && !validSortOrders.Contains(searchDto.SortOrder))
                {
                    return BadRequest(new { message = "Thứ tự sắp xếp không hợp lệ. Các giá trị hợp lệ: asc, desc" });
                }

                var result = await _menuService.SearchMenuItemsWithPaginationAsync(searchDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách món ăn", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo món ăn mới trong menu
        /// </summary>
        /// <param name="createDto">Thông tin món ăn cần tạo</param>
        /// <returns>Thông tin món ăn đã tạo thành công</returns>
        /// <response code="201">Tạo món ăn thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="500">Lỗi server khi tạo món ăn</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MenuItemv2Dto>> CreateNewMenuItem([FromBody] CreateMenuItemDto createDto)
        {
            try
            {
                // Validation cơ bản
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dữ liệu đầu vào không hợp lệ", errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
                }

                var menuItem = await _menuService.CreateMenuItemAsync(createDto);
                return CreatedAtAction(nameof(GetMenuItemById), new { id = menuItem.Id }, menuItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo món ăn", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật thông tin món ăn
        /// </summary>
        /// <param name="id">ID của món ăn cần cập nhật</param>
        /// <param name="updateDto">Thông tin mới của món ăn</param>
        /// <returns>Không có nội dung trả về</returns>
        /// <response code="204">Cập nhật món ăn thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="404">Không tìm thấy món ăn</response>
        /// <response code="500">Lỗi server khi cập nhật món ăn</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateMenuItem(Guid id, [FromBody] UpdateMenuItemDto updateDto)
        {
            try
            {
                // Validation cơ bản
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dữ liệu đầu vào không hợp lệ", errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
                }

                var result = await _menuService.UpdateMenuItemAsync(id, updateDto);
                if (!result)
                    return NotFound(new { message = "Không tìm thấy món ăn" });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật món ăn", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật trạng thái còn món/hết món
        /// </summary>
        /// <param name="id">ID của món ăn</param>
        /// <param name="isAvailable">true: còn món, false: hết món</param>
        /// <returns>Thông báo cập nhật thành công</returns>
        /// <response code="200">Cập nhật trạng thái thành công</response>
        /// <response code="404">Không tìm thấy món ăn</response>
        /// <response code="500">Lỗi server khi cập nhật trạng thái</response>
        [HttpPatch("{id}/availability-status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateMenuItemAvailabilityStatus(Guid id, [FromBody] bool isAvailable)
        {
            try
            {
                var result = await _menuService.UpdateAvailabilityAsync(id, isAvailable);
                if (!result)
                    return NotFound(new { message = "Không tìm thấy món ăn" });

                return Ok(new { message = isAvailable ? "Món ăn đã được đánh dấu là còn món" : "Món ăn đã được đánh dấu là hết món" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật trạng thái món ăn", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật giá món ăn
        /// </summary>
        /// <param name="id">ID của món ăn</param>
        /// <param name="newPrice">Giá mới của món ăn</param>
        /// <returns>Thông báo cập nhật giá thành công</returns>
        /// <response code="200">Cập nhật giá thành công</response>
        /// <response code="400">Giá không hợp lệ (âm)</response>
        /// <response code="404">Không tìm thấy món ăn</response>
        /// <response code="500">Lỗi server khi cập nhật giá</response>
        [HttpPatch("{id}/price")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateMenuItemPrice(Guid id, [FromBody] decimal newPrice)
        {
            try
            {
                // Validation: Kiểm tra giá không âm
                if (newPrice < 0)
                {
                    return BadRequest(new { message = "Giá món ăn không được âm" });
                }

                var result = await _menuService.UpdatePriceAsync(id, newPrice);
                if (!result)
                    return NotFound(new { message = "Không tìm thấy món ăn" });

                return Ok(new { message = $"Giá món ăn đã được cập nhật thành {newPrice:N0} VNĐ" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật giá món ăn", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật hình ảnh món ăn
        /// </summary>
        /// <param name="id">ID của món ăn</param>
        /// <param name="imageUrl">URL hình ảnh mới</param>
        /// <returns>Thông báo cập nhật hình ảnh thành công</returns>
        /// <response code="200">Cập nhật hình ảnh thành công</response>
        /// <response code="400">URL hình ảnh không hợp lệ</response>
        /// <response code="404">Không tìm thấy món ăn</response>
        /// <response code="500">Lỗi server khi cập nhật hình ảnh</response>
        [HttpPatch("{id}/image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateMenuItemImage(Guid id, [FromBody] string imageUrl)
        {
            try
            {
                // Validation: Kiểm tra URL không rỗng
                if (string.IsNullOrWhiteSpace(imageUrl))
                {
                    return BadRequest(new { message = "URL hình ảnh không được để trống" });
                }

                var result = await _menuService.UpdateImageAsync(id, imageUrl.Trim());
                if (!result)
                    return NotFound(new { message = "Không tìm thấy món ăn" });

                return Ok(new { message = "Hình ảnh món ăn đã được cập nhật" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật hình ảnh món ăn", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa món ăn khỏi menu
        /// </summary>
        /// <param name="id">ID của món ăn cần xóa</param>
        /// <returns>Thông báo xóa thành công</returns>
        /// <response code="200">Xóa món ăn thành công</response>
        /// <response code="400">Không thể xóa món ăn đã có trong đơn hàng</response>
        /// <response code="404">Không tìm thấy món ăn</response>
        /// <response code="500">Lỗi server khi xóa món ăn</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMenuItem(Guid id)
        {
            try
            {
                var result = await _menuService.DeleteMenuItemAsync(id);
                if (!result)
                    return NotFound(new { message = "Không tìm thấy món ăn" });

                return Ok(new { message = "Món ăn đã được xóa thành công" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa món ăn", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một món ăn theo ID
        /// </summary>
        /// <param name="id">ID của món ăn</param>
        /// <returns>Thông tin chi tiết món ăn</returns>
        /// <response code="200">Trả về thông tin món ăn thành công</response>
        /// <response code="404">Không tìm thấy món ăn</response>
        /// <response code="500">Lỗi server khi lấy thông tin món ăn</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MenuItemv2Dto>> GetMenuItemById(Guid id)
        {
            try
            {
                var menuItem = await _menuService.GetMenuItemByIdAsync(id);
                if (menuItem is null)
                    return NotFound(new { message = "Không tìm thấy món ăn" });

                return Ok(menuItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin món ăn", error = ex.Message });
            }
        }
    }
}
