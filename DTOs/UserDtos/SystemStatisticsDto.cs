namespace FoodioAPI.DTOs.UserDtos
{
    /// <summary>
    /// DTO cho thống kê toàn hệ thống
    /// </summary>
    public class SystemStatisticsDto
    {
        /// <summary>
        /// Tổng doanh thu toàn hệ thống
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Tổng số đơn hàng
        /// </summary>
        public int TotalOrders { get; set; }

        /// <summary>
        /// Tổng số người dùng
        /// </summary>
        public int TotalUsers { get; set; }

        /// <summary>
        /// Tổng số món ăn
        /// </summary>
        public int TotalMenuItems { get; set; }

        /// <summary>
        /// Tổng số danh mục
        /// </summary>
        public int TotalCategories { get; set; }

        /// <summary>
        /// Doanh thu trung bình mỗi đơn hàng
        /// </summary>
        public decimal AverageOrderValue { get; set; }

        /// <summary>
        /// Thống kê theo loại đơn hàng
        /// </summary>
        public List<RevenueByOrderTypeDto> RevenueByOrderType { get; set; } = new();

        /// <summary>
        /// Thống kê theo người dùng
        /// </summary>
        public List<RevenueByUserDto> RevenueByUser { get; set; } = new();

        /// <summary>
        /// Thống kê theo thời gian
        /// </summary>
        public List<RevenueByTimeDto> RevenueByTime { get; set; } = new();

        /// <summary>
        /// Thống kê theo tháng
        /// </summary>
        public List<RevenueByMonthDto> RevenueByMonth { get; set; } = new();

        /// <summary>
        /// Thống kê theo năm
        /// </summary>
        public List<RevenueByYearDto> RevenueByYear { get; set; } = new();
    }

    /// <summary>
    /// DTO cho thống kê doanh thu theo tháng
    /// </summary>
    public class RevenueByMonthDto
    {
        /// <summary>
        /// Năm
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Tháng
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Tên tháng
        /// </summary>
        public string MonthName { get; set; } = string.Empty;

        /// <summary>
        /// Tổng doanh thu
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Số lượng đơn hàng
        /// </summary>
        public int OrderCount { get; set; }
    }

    /// <summary>
    /// DTO cho thống kê doanh thu theo năm
    /// </summary>
    public class RevenueByYearDto
    {
        /// <summary>
        /// Năm
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Tổng doanh thu
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Số lượng đơn hàng
        /// </summary>
        public int OrderCount { get; set; }
    }

    /// <summary>
    /// DTO cho thống kê doanh thu theo khoảng thời gian
    /// </summary>
    public class RevenueByTimeRangeDto
    {
        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Tổng doanh thu
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Số lượng đơn hàng
        /// </summary>
        public int OrderCount { get; set; }

        /// <summary>
        /// Doanh thu trung bình mỗi ngày
        /// </summary>
        public decimal AverageDailyRevenue { get; set; }

        /// <summary>
        /// Số đơn hàng trung bình mỗi ngày
        /// </summary>
        public double AverageDailyOrders { get; set; }
    }

    /// <summary>
    /// DTO cho thống kê top người dùng
    /// </summary>
    public class TopUserDto
    {
        /// <summary>
        /// Tên người dùng
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Email người dùng
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Tổng doanh thu
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Số lượng đơn hàng
        /// </summary>
        public int OrderCount { get; set; }

        /// <summary>
        /// Doanh thu trung bình mỗi đơn hàng
        /// </summary>
        public decimal AverageOrderValue { get; set; }
    }

    /// <summary>
    /// DTO cho thống kê top loại đơn hàng
    /// </summary>
    public class TopOrderTypeDto
    {
        /// <summary>
        /// Tên loại đơn hàng
        /// </summary>
        public string OrderTypeName { get; set; } = string.Empty;

        /// <summary>
        /// Tổng doanh thu
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Số lượng đơn hàng
        /// </summary>
        public int OrderCount { get; set; }

        /// <summary>
        /// Tỷ lệ phần trăm so với tổng doanh thu
        /// </summary>
        public decimal RevenuePercentage { get; set; }
    }
} 