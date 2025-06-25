using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.DTOs.UserDtos
{
    /// <summary>
    /// DTO chứa các tham số tìm kiếm và lọc món ăn
    /// </summary>
    public class MenuItemSearchDto
    {
        /// <summary>
        /// Từ khóa tìm kiếm theo tên món ăn (không phân biệt hoa thường)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// ID danh mục để lọc món ăn theo danh mục
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Trạng thái còn món/hết món để lọc
        /// null: lấy tất cả, true: còn món, false: hết món
        /// </summary>
        public bool? IsAvailable { get; set; }

        /// <summary>
        /// Giá tối thiểu để lọc món ăn
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Giá tối thiểu không được âm")]
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// Giá tối đa để lọc món ăn
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Giá tối đa không được âm")]
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// Sắp xếp theo trường nào
        /// Các giá trị: "name", "price", "createdDate"
        /// </summary>
        public string? SortBy { get; set; } = "name";

        /// <summary>
        /// Thứ tự sắp xếp: "asc" (tăng dần) hoặc "desc" (giảm dần)
        /// </summary>
        public string? SortOrder { get; set; } = "asc";

        /// <summary>
        /// Số trang (bắt đầu từ 1)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Số trang phải lớn hơn 0")]
        public int Page { get; set; } = 1;

        /// <summary>
        /// Số lượng món ăn trên mỗi trang
        /// </summary>
        [Range(1, 100, ErrorMessage = "Số lượng món ăn trên trang phải từ 1-100")]
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Kiểm tra xem có tham số tìm kiếm nào không
        /// </summary>
        /// <returns>true nếu có tham số tìm kiếm</returns>
        public bool HasSearchCriteria()
        {
            return !string.IsNullOrWhiteSpace(SearchTerm) ||
                   CategoryId.HasValue ||
                   IsAvailable.HasValue ||
                   MinPrice.HasValue ||
                   MaxPrice.HasValue;
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của tham số giá
        /// </summary>
        /// <returns>true nếu tham số giá hợp lệ</returns>
        public bool IsPriceRangeValid()
        {
            if (MinPrice.HasValue && MaxPrice.HasValue)
            {
                return MinPrice.Value <= MaxPrice.Value;
            }
            return true;
        }
    }
} 