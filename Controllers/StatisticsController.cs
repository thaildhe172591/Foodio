using FoodioAPI.DTOs.UserDtos;
using FoodioAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodioAPI.Controllers
{
    /// <summary>
    /// Controller quản lý thống kê hệ thống
    /// Cung cấp các API để lấy thông tin thống kê toàn diện về doanh thu, đơn hàng, người dùng
    /// </summary>
    [Route("api/admin/statistics")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        /// <summary>
        /// Constructor với dependency injection
        /// </summary>
        /// <param name="statisticsService">Service thống kê</param>
        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        /// <summary>
        /// Lấy thống kê toàn hệ thống
        /// API này cung cấp tất cả thông tin thống kê quan trọng trong một lần gọi
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu lọc dữ liệu (tùy chọn)</param>
        /// <param name="endDate">Ngày kết thúc lọc dữ liệu (tùy chọn)</param>
        /// <returns>
        /// - 200 OK: Thống kê toàn hệ thống thành công
        /// - 500 Internal Server Error: Lỗi server
        /// </returns>
        /// <remarks>
        /// Ví dụ request:
        /// GET /api/admin/statistics/system?startDate=2024-01-01&endDate=2024-01-31
        /// 
        /// Response bao gồm:
        /// - Tổng doanh thu, số đơn hàng, số người dùng, món ăn, danh mục
        /// - Doanh thu trung bình mỗi đơn hàng
        /// - Thống kê chi tiết theo loại đơn hàng, người dùng, thời gian
        /// </remarks>
        [HttpGet("system")]
        [ProducesResponseType(typeof(SystemStatisticsDto), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<ActionResult<SystemStatisticsDto>> GetSystemStatistics(
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _statisticsService.GetSystemStatisticsAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Lỗi khi lấy thống kê hệ thống", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo loại đơn hàng
        /// Phân tích doanh thu theo từng loại đơn hàng (Dine-in, Takeaway, Delivery...)
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu (tùy chọn)</param>
        /// <param name="endDate">Ngày kết thúc (tùy chọn)</param>
        /// <returns>
        /// - 200 OK: Danh sách thống kê doanh thu theo loại đơn hàng
        /// - 500 Internal Server Error: Lỗi server
        /// </returns>
        /// <remarks>
        /// Ví dụ request:
        /// GET /api/admin/statistics/revenue/by-order-type?startDate=2024-01-01&endDate=2024-01-31
        /// 
        /// Kết quả được sắp xếp theo doanh thu giảm dần
        /// </remarks>
        [HttpGet("revenue/by-order-type")]
        [ProducesResponseType(typeof(IEnumerable<RevenueByOrderTypeDto>), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<ActionResult<IEnumerable<RevenueByOrderTypeDto>>> GetRevenueByOrderType(
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _statisticsService.GetRevenueByOrderTypeAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Lỗi khi lấy thống kê doanh thu theo loại đơn hàng", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo người dùng
        /// Phân tích doanh thu theo từng người dùng, có thể giới hạn số lượng top
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu (tùy chọn)</param>
        /// <param name="endDate">Ngày kết thúc (tùy chọn)</param>
        /// <param name="top">Số lượng top người dùng (mặc định: 10)</param>
        /// <returns>
        /// - 200 OK: Danh sách thống kê doanh thu theo người dùng
        /// - 500 Internal Server Error: Lỗi server
        /// </returns>
        /// <remarks>
        /// Ví dụ request:
        /// GET /api/admin/statistics/revenue/by-user?top=5&startDate=2024-01-01&endDate=2024-01-31
        /// 
        /// Kết quả được sắp xếp theo doanh thu giảm dần và giới hạn theo tham số top
        /// </remarks>
        [HttpGet("revenue/by-user")]
        [ProducesResponseType(typeof(IEnumerable<RevenueByUserDto>), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<ActionResult<IEnumerable<RevenueByUserDto>>> GetRevenueByUser(
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate,
            [FromQuery] int top = 10)
        {
            try
            {
                // Validation
                if (top <= 0)
                {
                    return BadRequest(new { message = "Tham số 'top' phải là số dương" });
                }

                var result = await _statisticsService.GetRevenueByUserAsync(startDate, endDate, top);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Lỗi khi lấy thống kê doanh thu theo người dùng", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo thời gian (ngày)
        /// Phân tích doanh thu theo từng ngày trong khoảng thời gian
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu (tùy chọn)</param>
        /// <param name="endDate">Ngày kết thúc (tùy chọn)</param>
        /// <returns>
        /// - 200 OK: Danh sách thống kê doanh thu theo thời gian
        /// - 500 Internal Server Error: Lỗi server
        /// </returns>
        /// <remarks>
        /// Ví dụ request:
        /// GET /api/admin/statistics/revenue/by-time?startDate=2024-01-01&endDate=2024-01-31
        /// 
        /// Kết quả được sắp xếp theo ngày tăng dần
        /// </remarks>
        [HttpGet("revenue/by-time")]
        [ProducesResponseType(typeof(IEnumerable<RevenueByTimeDto>), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<ActionResult<IEnumerable<RevenueByTimeDto>>> GetRevenueByTime(
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _statisticsService.GetRevenueByTimeAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Lỗi khi lấy thống kê doanh thu theo thời gian", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo tháng
        /// Phân tích doanh thu theo từng tháng trong năm
        /// </summary>
        /// <param name="year">Năm cần thống kê (mặc định: năm hiện tại)</param>
        /// <returns>
        /// - 200 OK: Danh sách thống kê doanh thu theo tháng
        /// - 500 Internal Server Error: Lỗi server
        /// </returns>
        /// <remarks>
        /// Ví dụ request:
        /// GET /api/admin/statistics/revenue/by-month?year=2024
        /// 
        /// Kết quả bao gồm tên tháng bằng tiếng Việt
        /// </remarks>
        [HttpGet("revenue/by-month")]
        [ProducesResponseType(typeof(IEnumerable<RevenueByMonthDto>), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<ActionResult<IEnumerable<RevenueByMonthDto>>> GetRevenueByMonth(
            [FromQuery] int? year = null)
        {
            try
            {
                // Validation
                if (year.HasValue && (year.Value < 1900 || year.Value > 2100))
                {
                    return BadRequest(new { message = "Năm phải trong khoảng 1900-2100" });
                }

                var result = await _statisticsService.GetRevenueByMonthAsync(year);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Lỗi khi lấy thống kê doanh thu theo tháng", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo năm
        /// Phân tích doanh thu theo từng năm
        /// </summary>
        /// <returns>
        /// - 200 OK: Danh sách thống kê doanh thu theo năm
        /// - 500 Internal Server Error: Lỗi server
        /// </returns>
        /// <remarks>
        /// Ví dụ request:
        /// GET /api/admin/statistics/revenue/by-year
        /// 
        /// Kết quả được sắp xếp theo năm tăng dần
        /// </remarks>
        [HttpGet("revenue/by-year")]
        [ProducesResponseType(typeof(IEnumerable<RevenueByYearDto>), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<ActionResult<IEnumerable<RevenueByYearDto>>> GetRevenueByYear()
        {
            try
            {
                var result = await _statisticsService.GetRevenueByYearAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Lỗi khi lấy thống kê doanh thu theo năm", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo khoảng thời gian cụ thể
        /// Phân tích tổng quan cho một khoảng thời gian với các chỉ số trung bình
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu (bắt buộc)</param>
        /// <param name="endDate">Ngày kết thúc (bắt buộc)</param>
        /// <returns>
        /// - 200 OK: Thống kê doanh thu theo khoảng thời gian
        /// - 400 Bad Request: Lỗi validation
        /// - 500 Internal Server Error: Lỗi server
        /// </returns>
        /// <remarks>
        /// Ví dụ request:
        /// GET /api/admin/statistics/revenue/by-time-range?startDate=2024-01-01&endDate=2024-01-31
        /// 
        /// Response bao gồm:
        /// - Tổng doanh thu và số đơn hàng
        /// - Doanh thu trung bình mỗi ngày
        /// - Số đơn hàng trung bình mỗi ngày
        /// </remarks>
        [HttpGet("revenue/by-time-range")]
        [ProducesResponseType(typeof(RevenueByTimeRangeDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<ActionResult<RevenueByTimeRangeDto>> GetRevenueByTimeRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            try
            {
                // Validation
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "Ngày bắt đầu không thể lớn hơn ngày kết thúc" });
                }

                var result = await _statisticsService.GetRevenueByTimeRangeAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Lỗi khi lấy thống kê doanh thu theo khoảng thời gian", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Lấy danh sách top người dùng có doanh thu cao nhất
        /// Xác định những người dùng có đóng góp doanh thu lớn nhất
        /// </summary>
        /// <param name="top">Số lượng top (mặc định: 10)</param>
        /// <param name="startDate">Ngày bắt đầu (tùy chọn)</param>
        /// <param name="endDate">Ngày kết thúc (tùy chọn)</param>
        /// <returns>
        /// - 200 OK: Danh sách top người dùng
        /// - 400 Bad Request: Lỗi validation
        /// - 500 Internal Server Error: Lỗi server
        /// </returns>
        /// <remarks>
        /// Ví dụ request:
        /// GET /api/admin/statistics/top-users?top=5&startDate=2024-01-01&endDate=2024-01-31
        /// 
        /// Response bao gồm:
        /// - Thông tin người dùng (tên, email)
        /// - Tổng doanh thu và số đơn hàng
        /// - Doanh thu trung bình mỗi đơn hàng
        /// </remarks>
        [HttpGet("top-users")]
        [ProducesResponseType(typeof(IEnumerable<TopUserDto>), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<ActionResult<IEnumerable<TopUserDto>>> GetTopUsers(
            [FromQuery] int top = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                // Validation
                if (top <= 0)
                {
                    return BadRequest(new { message = "Tham số 'top' phải là số dương" });
                }

                var result = await _statisticsService.GetTopUsersAsync(top, startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Lỗi khi lấy top người dùng", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Lấy danh sách top loại đơn hàng có doanh thu cao nhất
        /// Xác định những loại đơn hàng mang lại doanh thu lớn nhất
        /// </summary>
        /// <param name="top">Số lượng top (mặc định: 5)</param>
        /// <param name="startDate">Ngày bắt đầu (tùy chọn)</param>
        /// <param name="endDate">Ngày kết thúc (tùy chọn)</param>
        /// <returns>
        /// - 200 OK: Danh sách top loại đơn hàng
        /// - 400 Bad Request: Lỗi validation
        /// - 500 Internal Server Error: Lỗi server
        /// </returns>
        /// <remarks>
        /// Ví dụ request:
        /// GET /api/admin/statistics/top-order-types?top=3&startDate=2024-01-01&endDate=2024-01-31
        /// 
        /// Response bao gồm:
        /// - Tên loại đơn hàng
        /// - Tổng doanh thu và số đơn hàng
        /// - Tỷ lệ phần trăm so với tổng doanh thu
        /// </remarks>
        [HttpGet("top-order-types")]
        [ProducesResponseType(typeof(IEnumerable<TopOrderTypeDto>), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<ActionResult<IEnumerable<TopOrderTypeDto>>> GetTopOrderTypes(
            [FromQuery] int top = 5,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                // Validation
                if (top <= 0)
                {
                    return BadRequest(new { message = "Tham số 'top' phải là số dương" });
                }

                var result = await _statisticsService.GetTopOrderTypesAsync(top, startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Lỗi khi lấy top loại đơn hàng", 
                    error = ex.Message 
                });
            }
        }
    }
}
