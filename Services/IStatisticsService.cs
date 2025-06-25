using FoodioAPI.DTOs.UserDtos;

namespace FoodioAPI.Services
{
    /// <summary>
    /// Interface cho service quản lý thống kê hệ thống
    /// </summary>
    public interface IStatisticsService
    {
        /// <summary>
        /// Lấy thống kê toàn hệ thống
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu lọc dữ liệu (tùy chọn)</param>
        /// <param name="endDate">Ngày kết thúc lọc dữ liệu (tùy chọn)</param>
        /// <returns>Thống kê toàn hệ thống</returns>
        Task<SystemStatisticsDto> GetSystemStatisticsAsync(DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Lấy thống kê doanh thu theo loại đơn hàng
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu (tùy chọn)</param>
        /// <param name="endDate">Ngày kết thúc (tùy chọn)</param>
        /// <returns>Danh sách thống kê doanh thu theo loại đơn hàng</returns>
        Task<IEnumerable<RevenueByOrderTypeDto>> GetRevenueByOrderTypeAsync(DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Lấy thống kê doanh thu theo người dùng
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu (tùy chọn)</param>
        /// <param name="endDate">Ngày kết thúc (tùy chọn)</param>
        /// <param name="top">Số lượng top người dùng (mặc định: 10)</param>
        /// <returns>Danh sách thống kê doanh thu theo người dùng</returns>
        Task<IEnumerable<RevenueByUserDto>> GetRevenueByUserAsync(DateTime? startDate, DateTime? endDate, int top = 10);

        /// <summary>
        /// Lấy thống kê doanh thu theo thời gian (ngày)
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu (tùy chọn)</param>
        /// <param name="endDate">Ngày kết thúc (tùy chọn)</param>
        /// <returns>Danh sách thống kê doanh thu theo thời gian</returns>
        Task<IEnumerable<RevenueByTimeDto>> GetRevenueByTimeAsync(DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Lấy thống kê doanh thu theo tháng
        /// </summary>
        /// <param name="year">Năm cần thống kê (mặc định: năm hiện tại)</param>
        /// <returns>Danh sách thống kê doanh thu theo tháng</returns>
        Task<IEnumerable<RevenueByMonthDto>> GetRevenueByMonthAsync(int? year = null);

        /// <summary>
        /// Lấy thống kê doanh thu theo năm
        /// </summary>
        /// <returns>Danh sách thống kê doanh thu theo năm</returns>
        Task<IEnumerable<RevenueByYearDto>> GetRevenueByYearAsync();

        /// <summary>
        /// Lấy thống kê doanh thu theo khoảng thời gian
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <returns>Thống kê doanh thu theo khoảng thời gian</returns>
        Task<RevenueByTimeRangeDto> GetRevenueByTimeRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Lấy danh sách top người dùng có doanh thu cao nhất
        /// </summary>
        /// <param name="top">Số lượng top (mặc định: 10)</param>
        /// <param name="startDate">Ngày bắt đầu (tùy chọn)</param>
        /// <param name="endDate">Ngày kết thúc (tùy chọn)</param>
        /// <returns>Danh sách top người dùng</returns>
        Task<IEnumerable<TopUserDto>> GetTopUsersAsync(int top = 10, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Lấy danh sách top loại đơn hàng có doanh thu cao nhất
        /// </summary>
        /// <param name="top">Số lượng top (mặc định: 5)</param>
        /// <param name="startDate">Ngày bắt đầu (tùy chọn)</param>
        /// <param name="endDate">Ngày kết thúc (tùy chọn)</param>
        /// <returns>Danh sách top loại đơn hàng</returns>
        Task<IEnumerable<TopOrderTypeDto>> GetTopOrderTypesAsync(int top = 5, DateTime? startDate = null, DateTime? endDate = null);
    }
} 