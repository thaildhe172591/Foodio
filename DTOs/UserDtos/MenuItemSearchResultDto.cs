using FoodioAPI.DTOs.Menu;

namespace FoodioAPI.DTOs.UserDtos
{
    /// <summary>
    /// DTO chứa kết quả tìm kiếm món ăn với phân trang
    /// </summary>
    public class MenuItemSearchResultDto
    {
        /// <summary>
        /// Danh sách món ăn trong trang hiện tại
        /// </summary>
        public IEnumerable<MenuItemv2Dto> Items { get; set; } = new List<MenuItemv2Dto>();

        /// <summary>
        /// Tổng số món ăn thỏa mãn điều kiện tìm kiếm
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Số trang hiện tại
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Số lượng món ăn trên mỗi trang
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Có trang tiếp theo không
        /// </summary>
        public bool HasNextPage { get; set; }

        /// <summary>
        /// Có trang trước đó không
        /// </summary>
        public bool HasPreviousPage { get; set; }

        /// <summary>
        /// Thông tin về tham số tìm kiếm đã sử dụng
        /// </summary>
        public MenuItemSearchDto SearchCriteria { get; set; } = new MenuItemSearchDto();

        /// <summary>
        /// Thống kê nhanh về kết quả tìm kiếm
        /// </summary>
        public SearchStatistics Statistics { get; set; } = new SearchStatistics();
    }

    /// <summary>
    /// Thống kê về kết quả tìm kiếm
    /// </summary>
    public class SearchStatistics
    {
        /// <summary>
        /// Số món ăn còn món
        /// </summary>
        public int AvailableCount { get; set; }

        /// <summary>
        /// Số món ăn hết món
        /// </summary>
        public int UnavailableCount { get; set; }

        /// <summary>
        /// Giá trung bình của các món ăn
        /// </summary>
        public decimal AveragePrice { get; set; }

        /// <summary>
        /// Giá thấp nhất
        /// </summary>
        public decimal MinPrice { get; set; }

        /// <summary>
        /// Giá cao nhất
        /// </summary>
        public decimal MaxPrice { get; set; }
    }
}