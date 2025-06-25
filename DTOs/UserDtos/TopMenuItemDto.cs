namespace FoodioAPI.DTOs.UserDtos
{
    /// <summary>
    /// DTO cho thống kê top món ăn bán chạy
    /// </summary>
    public class TopMenuItemDto
    {
        /// <summary>
        /// Tên món ăn
        /// </summary>
        public string MenuItemName { get; set; } = string.Empty;

        /// <summary>
        /// Tổng doanh thu thu được từ món ăn
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Số lượng món ăn đã bán
        /// </summary>
        public int QuantitySold { get; set; }

        /// <summary>
        /// Tỷ lệ doanh thu của món ăn so với tổng doanh thu (%)
        /// </summary>
        public decimal RevenuePercentage { get; set; }
    }
} 