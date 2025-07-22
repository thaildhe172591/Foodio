using FoodioAPI.DTOs.KitchenStaff; // Đảm bảo thêm namespace cho DTO

using FoodioAPI.DTOs.UserDtos;

namespace FoodioAPI.Services
{
    /// <summary>
    /// Interface cho service quản lý danh mục
    /// Cung cấp các business logic cho việc CRUD danh mục
    /// </summary>
    public interface ICategoryService
    {
        //Hiếu làm 
        Task<List<OrderWithDetails>> GetOrdersWithStatusPendingAsync();        
        Task<List<OrderWithDetails>> GetPendingHotDishOrdersAsync();           
        Task<List<OrderWithDetails>> GetPendingDrinksDishOrdersAsync();

        Task<List<OrderWithDetails>> GetOrdersWithStatusCookingColdAsync();
        Task<List<OrderWithDetails>> GetOrdersWithStatusCookingHotAsync();
        Task<List<OrderWithDetails>> GetOrdersWithStatusCookingDrinksAsync();
        Task<List<OrderWithDetails>> GetAllOrderReadyToServeAsync();
        Task<bool> UpdateOrderItemStatusAsync(Guid orderItemId, string newStatusCode);
        //đến đây


        #region Read Operations - Lấy thông tin danh mục

        /// <summary>
        /// Lấy tất cả danh mục với thông tin số lượng món ăn
        /// </summary>
        /// <returns>Danh sách tất cả danh mục</returns>
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();

        /// <summary>
        /// Tìm kiếm và lọc danh mục với phân trang
        /// </summary>
        /// <param name="searchDto">Tham số tìm kiếm và lọc</param>
        /// <returns>Kết quả tìm kiếm với phân trang và thống kê</returns>
        Task<CategorySearchResultDto> SearchCategoriesWithPaginationAsync(CategorySearchDto searchDto);

        /// <summary>
        /// Lấy thông tin chi tiết của một danh mục theo ID
        /// </summary>
        /// <param name="id">ID của danh mục</param>
        /// <returns>Thông tin chi tiết danh mục, null nếu không tìm thấy</returns>
        Task<CategoryDto?> GetCategoryByIdAsync(Guid id);

        /// <summary>
        /// Tìm kiếm danh mục theo tên
        /// </summary>
        /// <param name="name">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách danh mục khớp với từ khóa</returns>
        /// <exception cref="ArgumentException">Khi từ khóa tìm kiếm rỗng</exception>
        Task<IEnumerable<CategoryDto>> SearchCategoriesAsync(string name);

        #endregion

        #region Create, Update, Delete Operations - CRUD cơ bản

        /// <summary>
        /// Tạo danh mục mới
        /// </summary>
        /// <param name="dto">Thông tin danh mục cần tạo</param>
        /// <returns>Thông tin danh mục đã tạo thành công</returns>
        /// <exception cref="InvalidOperationException">Khi tên danh mục đã trùng</exception>
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);

        /// <summary>
        /// Cập nhật thông tin danh mục
        /// </summary>
        /// <param name="id">ID của danh mục cần cập nhật</param>
        /// <param name="dto">Thông tin mới của danh mục</param>
        /// <returns>true nếu cập nhật thành công, false nếu không tìm thấy danh mục</returns>
        /// <exception cref="InvalidOperationException">Khi tên danh mục đã trùng</exception>
        Task<bool> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto);

        /// <summary>
        /// Xóa danh mục
        /// </summary>
        /// <param name="id">ID của danh mục cần xóa</param>
        /// <returns>true nếu xóa thành công, false nếu không tìm thấy danh mục</returns>
        /// <exception cref="InvalidOperationException">Khi không thể xóa danh mục đã có món ăn</exception>
        Task<bool> DeleteCategoryAsync(Guid id);

        #endregion

        #region Validation Methods - Các method validation

        /// <summary>
        /// Kiểm tra tên danh mục đã tồn tại chưa
        /// </summary>
        /// <param name="name">Tên danh mục cần kiểm tra</param>
        /// <param name="excludeId">ID danh mục cần loại trừ (dùng cho update)</param>
        /// <returns>true nếu tên đã tồn tại, false nếu chưa</returns>
        Task<bool> ValidateCategoryNameExistsAsync(string name, Guid? excludeId = null);

        /// <summary>
        /// Kiểm tra danh mục có thể xóa được không (không có món ăn)
        /// </summary>
        /// <param name="id">ID của danh mục cần kiểm tra</param>
        /// <returns>true nếu có thể xóa, false nếu không thể xóa</returns>
        Task<bool> ValidateCategoryCanBeDeletedAsync(Guid id);

        #endregion
    }
} 