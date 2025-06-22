namespace FoodioAPI.DTOs.UserDtos
{
    /// <summary>
    /// DTO cho thông tin danh mục
    /// </summary>
    public class CategoryDto
    {
        /// <summary>
        /// ID của danh mục
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Tên danh mục
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Số lượng món ăn trong danh mục
        /// </summary>
        public int MenuItemCount { get; set; }
    }

    /// <summary>
    /// DTO để tạo danh mục mới
    /// </summary>
    public class CreateCategoryDto
    {
        /// <summary>
        /// Tên danh mục (bắt buộc, tối đa 100 ký tự)
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO để cập nhật danh mục
    /// </summary>
    public class UpdateCategoryDto
    {
        /// <summary>
        /// Tên danh mục mới (bắt buộc, tối đa 100 ký tự)
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO cho tham số tìm kiếm danh mục
    /// </summary>
    public class CategorySearchDto
    {
        /// <summary>
        /// Từ khóa tìm kiếm theo tên danh mục
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Sắp xếp theo trường (name, menuItemCount)
        /// </summary>
        public string? SortBy { get; set; } = "name";

        /// <summary>
        /// Thứ tự sắp xếp (asc, desc)
        /// </summary>
        public string? SortOrder { get; set; } = "asc";

        /// <summary>
        /// Số trang (bắt đầu từ 1)
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Số lượng danh mục trên mỗi trang
        /// </summary>
        public int PageSize { get; set; } = 20;
    }

    /// <summary>
    /// DTO cho kết quả tìm kiếm danh mục với phân trang
    /// </summary>
    public class CategorySearchResultDto
    {
        /// <summary>
        /// Danh sách danh mục trong trang hiện tại
        /// </summary>
        public IEnumerable<CategoryDto> Items { get; set; } = new List<CategoryDto>();

        /// <summary>
        /// Tổng số danh mục thỏa mãn điều kiện tìm kiếm
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Số trang hiện tại
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Số lượng danh mục trên mỗi trang
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
        public CategorySearchDto SearchCriteria { get; set; } = new CategorySearchDto();

        /// <summary>
        /// Thống kê nhanh về kết quả tìm kiếm
        /// </summary>
        public CategoryStatistics Statistics { get; set; } = new CategoryStatistics();
    }

    /// <summary>
    /// Thống kê về danh mục
    /// </summary>
    public class CategoryStatistics
    {
        /// <summary>
        /// Tổng số danh mục
        /// </summary>
        public int TotalCategories { get; set; }

        /// <summary>
        /// Số danh mục có món ăn
        /// </summary>
        public int CategoriesWithItems { get; set; }

        /// <summary>
        /// Số danh mục không có món ăn
        /// </summary>
        public int EmptyCategories { get; set; }

        /// <summary>
        /// Tổng số món ăn trong tất cả danh mục
        /// </summary>
        public int TotalMenuItems { get; set; }

        /// <summary>
        /// Số món ăn trung bình mỗi danh mục
        /// </summary>
        public double AverageItemsPerCategory { get; set; }
    }
} 