using FoodioAPI.DTOs.UserDtos;
using FoodioAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodioAPI.Controllers
{
    /// <summary>
    /// Controller quản lý danh mục món ăn
    /// Cung cấp các API CRUD và quản lý danh mục
    /// </summary>
    [Route("api/admin/categories")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// API tổng hợp để lấy danh sách danh mục với đầy đủ chức năng:
        /// - Lấy tất cả danh mục
        /// - Tìm kiếm theo tên
        /// - Sắp xếp theo nhiều tiêu chí
        /// - Phân trang
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm theo tên danh mục</param>
        /// <param name="sortBy">Sắp xếp theo trường (name, menuItemCount) - mặc định: "name"</param>
        /// <param name="sortOrder">Thứ tự sắp xếp (asc, desc) - mặc định: "asc"</param>
        /// <param name="page">Số trang - mặc định: 1</param>
        /// <param name="pageSize">Số lượng danh mục trên trang - mặc định: 20, tối đa: 100</param>
        /// <returns>Danh sách danh mục với thông tin phân trang và thống kê</returns>
        /// <response code="200">Trả về danh sách danh mục thành công</response>
        /// <response code="400">Tham số tìm kiếm không hợp lệ</response>
        /// <response code="500">Lỗi server khi lấy danh sách danh mục</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategorySearchResultDto>> GetCategories(
            [FromQuery] string? searchTerm = null,
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
                    return BadRequest(new { message = "Số lượng danh mục trên trang phải từ 1-100" });
                }

                // Tạo DTO tìm kiếm từ các tham số
                var searchDto = new CategorySearchDto
                {
                    SearchTerm = searchTerm?.Trim(),
                    SortBy = sortBy?.ToLower(),
                    SortOrder = sortOrder?.ToLower(),
                    Page = page,
                    PageSize = pageSize
                };

                // Validation cho SortBy
                var validSortFields = new[] { "name", "menuitemcount" };
                if (!string.IsNullOrEmpty(searchDto.SortBy) && !validSortFields.Contains(searchDto.SortBy))
                {
                    return BadRequest(new { message = "Trường sắp xếp không hợp lệ. Các giá trị hợp lệ: name, menuItemCount" });
                }

                // Validation cho SortOrder
                var validSortOrders = new[] { "asc", "desc" };
                if (!string.IsNullOrEmpty(searchDto.SortOrder) && !validSortOrders.Contains(searchDto.SortOrder))
                {
                    return BadRequest(new { message = "Thứ tự sắp xếp không hợp lệ. Các giá trị hợp lệ: asc, desc" });
                }

                var result = await _categoryService.SearchCategoriesWithPaginationAsync(searchDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách danh mục", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một danh mục theo ID
        /// </summary>
        /// <param name="id">ID của danh mục</param>
        /// <returns>Thông tin chi tiết danh mục</returns>
        /// <response code="200">Trả về thông tin danh mục thành công</response>
        /// <response code="404">Không tìm thấy danh mục</response>
        /// <response code="500">Lỗi server khi lấy thông tin danh mục</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(Guid id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category is null)
                    return NotFound(new { message = "Không tìm thấy danh mục" });

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin danh mục", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo danh mục mới
        /// </summary>
        /// <param name="createDto">Thông tin danh mục cần tạo</param>
        /// <returns>Thông tin danh mục đã tạo thành công</returns>
        /// <response code="201">Tạo danh mục thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="500">Lỗi server khi tạo danh mục</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryDto createDto)
        {
            try
            {
                // Validation cơ bản
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dữ liệu đầu vào không hợp lệ", errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
                }

                var category = await _categoryService.CreateCategoryAsync(createDto);
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
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
                return StatusCode(500, new { message = "Lỗi khi tạo danh mục", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật thông tin danh mục
        /// </summary>
        /// <param name="id">ID của danh mục cần cập nhật</param>
        /// <param name="updateDto">Thông tin mới của danh mục</param>
        /// <returns>Không có nội dung trả về</returns>
        /// <response code="204">Cập nhật danh mục thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="404">Không tìm thấy danh mục</response>
        /// <response code="500">Lỗi server khi cập nhật danh mục</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto updateDto)
        {
            try
            {
                // Validation cơ bản
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dữ liệu đầu vào không hợp lệ", errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
                }

                var result = await _categoryService.UpdateCategoryAsync(id, updateDto);
                if (!result)
                    return NotFound(new { message = "Không tìm thấy danh mục" });

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
                return StatusCode(500, new { message = "Lỗi khi cập nhật danh mục", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa danh mục
        /// </summary>
        /// <param name="id">ID của danh mục cần xóa</param>
        /// <returns>Thông báo xóa thành công</returns>
        /// <response code="200">Xóa danh mục thành công</response>
        /// <response code="400">Không thể xóa danh mục đã có món ăn</response>
        /// <response code="404">Không tìm thấy danh mục</response>
        /// <response code="500">Lỗi server khi xóa danh mục</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (!result)
                    return NotFound(new { message = "Không tìm thấy danh mục" });

                return Ok(new { message = "Danh mục đã được xóa thành công" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa danh mục", error = ex.Message });
            }
        }

        /// <summary>
        /// Tìm kiếm danh mục theo tên (API đơn giản)
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách danh mục khớp với từ khóa</returns>
        /// <response code="200">Tìm kiếm thành công</response>
        /// <response code="400">Từ khóa tìm kiếm không hợp lệ</response>
        /// <response code="500">Lỗi server khi tìm kiếm</response>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> SearchCategories([FromQuery] string searchTerm)
        {
            try
            {
                // Validation: Kiểm tra từ khóa tìm kiếm không rỗng
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Từ khóa tìm kiếm không được để trống" });
                }

                var categories = await _categoryService.SearchCategoriesAsync(searchTerm.Trim());
                return Ok(categories);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tìm kiếm danh mục", error = ex.Message });
            }
        }
    }
} 