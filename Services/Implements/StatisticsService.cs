using FoodioAPI.Database;
using FoodioAPI.DTOs.UserDtos;
using FoodioAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodioAPI.Services.Implements
{
    /// <summary>
    /// Service quản lý thống kê hệ thống
    /// </summary>
    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext _context;

        public StatisticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy thống kê toàn hệ thống
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu lọc dữ liệu (tùy chọn)</param>
        /// <param name="endDate">Ngày kết thúc lọc dữ liệu (tùy chọn)</param>
        /// <returns>Thống kê toàn hệ thống</returns>
        public async Task<SystemStatisticsDto> GetSystemStatisticsAsync(DateTime? startDate, DateTime? endDate)
        {
            // Xây dựng query cơ bản cho đơn hàng
            var query = _context.Orders.AsQueryable();

            // Lọc theo khoảng thời gian nếu có
            if (startDate.HasValue)
                query = query.Where(o => o.CreatedAt >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(o => o.CreatedAt <= endDate.Value);

            // Thống kê tổng quan
            var totalRevenue = await query.SumAsync(o => o.Total);
            var totalOrders = await query.CountAsync();
            var totalUsers = await _context.Users.CountAsync();
            var totalMenuItems = await _context.MenuItems.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();
            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            // Thống kê theo loại đơn hàng
            var revenueByOrderType = await GetRevenueByOrderTypeInternalAsync(query);

            // Thống kê theo người dùng
            var revenueByUser = await GetRevenueByUserInternalAsync(query);

            // Thống kê theo ngày
            var revenueByTime = await GetRevenueByTimeInternalAsync(query);

            // Thống kê theo tháng
            var revenueByMonth = await GetRevenueByMonthInternalAsync(query);

            // Thống kê theo năm
            var revenueByYear = await GetRevenueByYearInternalAsync(query);

            return new SystemStatisticsDto
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalUsers = totalUsers,
                TotalMenuItems = totalMenuItems,
                TotalCategories = totalCategories,
                AverageOrderValue = averageOrderValue,
                RevenueByOrderType = revenueByOrderType.ToList(),
                RevenueByUser = revenueByUser.ToList(),
                RevenueByTime = revenueByTime.ToList(),
                RevenueByMonth = revenueByMonth.ToList(),
                RevenueByYear = revenueByYear.ToList()
            };
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo loại đơn hàng
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu (tùy chọn)</param>
        /// <param name="endDate">Ngày kết thúc (tùy chọn)</param>
        /// <returns>Danh sách thống kê doanh thu theo loại đơn hàng</returns>
        public async Task<IEnumerable<RevenueByOrderTypeDto>> GetRevenueByOrderTypeAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = BuildBaseQuery(startDate, endDate);
            return await GetRevenueByOrderTypeInternalAsync(query);
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo người dùng
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu (tùy chọn)</param>
        /// <param name="endDate">Ngày kết thúc (tùy chọn)</param>
        /// <param name="top">Số lượng top người dùng (mặc định: 10)</param>
        /// <returns>Danh sách thống kê doanh thu theo người dùng</returns>
        public async Task<IEnumerable<RevenueByUserDto>> GetRevenueByUserAsync(DateTime? startDate, DateTime? endDate, int top = 10)
        {
            var query = BuildBaseQuery(startDate, endDate);
            return await GetRevenueByUserInternalAsync(query, top);
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo thời gian (ngày)
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu (tùy chọn)</param>
        /// <param name="endDate">Ngày kết thúc (tùy chọn)</param>
        /// <returns>Danh sách thống kê doanh thu theo thời gian</returns>
        public async Task<IEnumerable<RevenueByTimeDto>> GetRevenueByTimeAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = BuildBaseQuery(startDate, endDate);
            return await GetRevenueByTimeInternalAsync(query);
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo tháng
        /// </summary>
        /// <param name="year">Năm cần thống kê (mặc định: năm hiện tại)</param>
        /// <returns>Danh sách thống kê doanh thu theo tháng</returns>
        public async Task<IEnumerable<RevenueByMonthDto>> GetRevenueByMonthAsync(int? year = null)
        {
            var targetYear = year ?? DateTime.Now.Year;
            var query = _context.Orders.Where(o => o.CreatedAt.Year == targetYear);
            return await GetRevenueByMonthInternalAsync(query);
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo năm
        /// </summary>
        /// <returns>Danh sách thống kê doanh thu theo năm</returns>
        public async Task<IEnumerable<RevenueByYearDto>> GetRevenueByYearAsync()
        {
            var query = _context.Orders.AsQueryable();
            return await GetRevenueByYearInternalAsync(query);
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo khoảng thời gian
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <returns>Thống kê doanh thu theo khoảng thời gian</returns>
        public async Task<RevenueByTimeRangeDto> GetRevenueByTimeRangeAsync(DateTime startDate, DateTime endDate)
        {
            var query = _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate);

            var totalRevenue = await query.SumAsync(o => o.Total);
            var orderCount = await query.CountAsync();
            var daysDiff = (endDate - startDate).Days + 1;
            var averageDailyRevenue = daysDiff > 0 ? totalRevenue / daysDiff : 0;
            var averageDailyOrders = daysDiff > 0 ? (double)orderCount / daysDiff : 0;

            return new RevenueByTimeRangeDto
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRevenue = totalRevenue,
                OrderCount = orderCount,
                AverageDailyRevenue = averageDailyRevenue,
                AverageDailyOrders = averageDailyOrders
            };
        }

        /// <summary>
        /// Lấy danh sách top người dùng có doanh thu cao nhất
        /// </summary>
        /// <param name="top">Số lượng top (mặc định: 10)</param>
        /// <param name="startDate">Ngày bắt đầu (tùy chọn)</param>
        /// <param name="endDate">Ngày kết thúc (tùy chọn)</param>
        /// <returns>Danh sách top người dùng</returns>
        public async Task<IEnumerable<TopUserDto>> GetTopUsersAsync(int top = 10, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = BuildBaseQuery(startDate, endDate);

            return await query
                .Include(o => o.User)
                .GroupBy(o => new { o.User.UserName, o.User.Email })
                .Select(g => new TopUserDto
                {
                    UserName = g.Key.UserName!,
                    Email = g.Key.Email!,
                    TotalRevenue = g.Sum(o => o.Total),
                    OrderCount = g.Count(),
                    AverageOrderValue = g.Count() > 0 ? g.Sum(o => o.Total) / g.Count() : 0
                })
                .OrderByDescending(u => u.TotalRevenue)
                .Take(top)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy danh sách top loại đơn hàng có doanh thu cao nhất
        /// </summary>
        /// <param name="top">Số lượng top (mặc định: 5)</param>
        /// <param name="startDate">Ngày bắt đầu (tùy chọn)</param>
        /// <param name="endDate">Ngày kết thúc (tùy chọn)</param>
        /// <returns>Danh sách top loại đơn hàng</returns>
        public async Task<IEnumerable<TopOrderTypeDto>> GetTopOrderTypesAsync(int top = 5, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = BuildBaseQuery(startDate, endDate);
            var totalSystemRevenue = await query.SumAsync(o => o.Total);

            return await query
                .Include(o => o.OrderType)
                .GroupBy(o => o.OrderType.Name)
                .Select(g => new TopOrderTypeDto
                {
                    OrderTypeName = g.Key,
                    TotalRevenue = g.Sum(o => o.Total),
                    OrderCount = g.Count(),
                    RevenuePercentage = totalSystemRevenue > 0 ? (g.Sum(o => o.Total) / totalSystemRevenue) * 100 : 0
                })
                .OrderByDescending(ot => ot.TotalRevenue)
                .Take(top)
                .ToListAsync();
        }

        #region Private Methods

        /// <summary>
        /// Xây dựng query cơ bản với filter thời gian
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <returns>Query đã được filter</returns>
        private IQueryable<Order> BuildBaseQuery(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Orders.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(o => o.CreatedAt >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(o => o.CreatedAt <= endDate.Value);

            return query;
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo loại đơn hàng (internal)
        /// </summary>
        /// <param name="query">Query cơ bản</param>
        /// <returns>Danh sách thống kê</returns>
        private async Task<IEnumerable<RevenueByOrderTypeDto>> GetRevenueByOrderTypeInternalAsync(IQueryable<Order> query)
        {
            return await query
                .Include(o => o.OrderType)
                .GroupBy(o => o.OrderType.Name)
                .Select(g => new RevenueByOrderTypeDto
                {
                    OrderTypeName = g.Key,
                    TotalRevenue = g.Sum(o => o.Total),
                    OrderCount = g.Count()
                })
                .OrderByDescending(r => r.TotalRevenue)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo người dùng (internal)
        /// </summary>
        /// <param name="query">Query cơ bản</param>
        /// <param name="top">Số lượng top</param>
        /// <returns>Danh sách thống kê</returns>
        private async Task<IEnumerable<RevenueByUserDto>> GetRevenueByUserInternalAsync(IQueryable<Order> query, int? top = null)
        {
            var result = query
                .Include(o => o.User)
                .GroupBy(o => o.User.UserName)
                .Select(g => new RevenueByUserDto
                {
                    UserName = g.Key!,
                    TotalRevenue = g.Sum(o => o.Total),
                    OrderCount = g.Count()
                })
                .OrderByDescending(r => r.TotalRevenue);

            if (top.HasValue)
                result = (IOrderedQueryable<RevenueByUserDto>)result.Take(top.Value);

            return await result.ToListAsync();
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo thời gian (internal)
        /// </summary>
        /// <param name="query">Query cơ bản</param>
        /// <returns>Danh sách thống kê</returns>
        private async Task<IEnumerable<RevenueByTimeDto>> GetRevenueByTimeInternalAsync(IQueryable<Order> query)
        {
            return await query
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new RevenueByTimeDto
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(o => o.Total),
                    OrderCount = g.Count()
                })
                .OrderBy(g => g.Date)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo tháng (internal)
        /// </summary>
        /// <param name="query">Query cơ bản</param>
        /// <returns>Danh sách thống kê</returns>
        private async Task<IEnumerable<RevenueByMonthDto>> GetRevenueByMonthInternalAsync(IQueryable<Order> query)
        {
            return await query
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new RevenueByMonthDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                    TotalRevenue = g.Sum(o => o.Total),
                    OrderCount = g.Count()
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo năm (internal)
        /// </summary>
        /// <param name="query">Query cơ bản</param>
        /// <returns>Danh sách thống kê</returns>
        private async Task<IEnumerable<RevenueByYearDto>> GetRevenueByYearInternalAsync(IQueryable<Order> query)
        {
            return await query
                .GroupBy(o => o.CreatedAt.Year)
                .Select(g => new RevenueByYearDto
                {
                    Year = g.Key,
                    TotalRevenue = g.Sum(o => o.Total),
                    OrderCount = g.Count()
                })
                .OrderBy(g => g.Year)
                .ToListAsync();
        }

        #endregion
    }
}