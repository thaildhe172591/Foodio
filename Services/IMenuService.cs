using FoodioAPI.DTOs.Menu;
using FoodioAPI.DTOs.UserDtos;

namespace FoodioAPI.Services
{
    /// <summary>
    /// Interface cho service quản lý món ăn trong menu
    /// Cung cấp các business logic cho việc CRUD và quản lý trạng thái món ăn
    /// </summary>
    public interface IMenuService
    {
        #region Read Operations - Lấy thông tin món ăn

        /// <summary>
        /// Lấy danh sách món ăn với bộ lọc tùy chỉnh
        /// </summary>
        /// <param name="filter">Bộ lọc cho việc tìm kiếm và phân loại món ăn</param>
        /// <returns>Danh sách món ăn khớp với bộ lọc</returns>
        Task<IEnumerable<MenuItemv2Dto>> GetMenuItemsAsync(FilterMenuItemDto filter);

        /// <summary>
        /// Tìm kiếm và lọc món ăn với nhiều tham số (tên, danh mục, trạng thái, giá, phân trang, sắp xếp)
        /// </summary>
        /// <param name="searchDto">Tham số tìm kiếm và lọc</param>
        /// <returns>Kết quả tìm kiếm với phân trang và thống kê</returns>
        Task<MenuItemSearchResultDto> SearchMenuItemsWithPaginationAsync(MenuItemSearchDto searchDto);

        /// <summary>
        /// Lấy thông tin chi tiết của một món ăn theo ID
        /// </summary>
        /// <param name="id">ID của món ăn</param>
        /// <returns>Thông tin chi tiết món ăn, null nếu không tìm thấy</returns>
        Task<MenuItemv2Dto?> GetMenuItemByIdAsync(Guid id);

        #endregion

        #region Create, Update, Delete Operations - CRUD cơ bản

        /// <summary>
        /// Tạo món ăn mới trong menu
        /// </summary>
        /// <param name="dto">Thông tin món ăn cần tạo</param>
        /// <returns>Thông tin món ăn đã tạo thành công</returns>
        /// <exception cref="InvalidOperationException">Khi danh mục không tồn tại hoặc tên món ăn đã trùng</exception>
        Task<MenuItemv2Dto> CreateMenuItemAsync(CreateMenuItemDto dto);

        /// <summary>
        /// Cập nhật thông tin món ăn
        /// </summary>
        /// <param name="id">ID của món ăn cần cập nhật</param>
        /// <param name="dto">Thông tin mới của món ăn</param>
        /// <returns>true nếu cập nhật thành công, false nếu không tìm thấy món ăn</returns>
        /// <exception cref="InvalidOperationException">Khi danh mục không tồn tại hoặc tên món ăn đã trùng</exception>
        Task<bool> UpdateMenuItemAsync(Guid id, UpdateMenuItemDto dto);

        /// <summary>
        /// Xóa món ăn khỏi menu
        /// </summary>
        /// <param name="id">ID của món ăn cần xóa</param>
        /// <returns>true nếu xóa thành công, false nếu không tìm thấy món ăn</returns>
        /// <exception cref="InvalidOperationException">Khi không thể xóa món ăn đã có trong đơn hàng</exception>
        Task<bool> DeleteMenuItemAsync(Guid id);

        #endregion

        #region Partial Update Operations - Cập nhật từng phần

        /// <summary>
        /// Cập nhật trạng thái còn món/hết món
        /// </summary>
        /// <param name="id">ID của món ăn</param>
        /// <param name="isAvailable">true: còn món, false: hết món</param>
        /// <returns>true nếu cập nhật thành công, false nếu không tìm thấy món ăn</returns>
        Task<bool> UpdateAvailabilityAsync(Guid id, bool isAvailable);

        /// <summary>
        /// Cập nhật giá món ăn
        /// </summary>
        /// <param name="id">ID của món ăn</param>
        /// <param name="price">Giá mới của món ăn</param>
        /// <returns>true nếu cập nhật thành công, false nếu không tìm thấy món ăn</returns>
        /// <exception cref="ArgumentException">Khi giá âm</exception>
        Task<bool> UpdatePriceAsync(Guid id, decimal price);

        /// <summary>
        /// Cập nhật hình ảnh món ăn
        /// </summary>
        /// <param name="id">ID của món ăn</param>
        /// <param name="imageUrl">URL hình ảnh mới</param>
        /// <returns>true nếu cập nhật thành công, false nếu không tìm thấy món ăn</returns>
        Task<bool> UpdateImageAsync(Guid id, string imageUrl);

        #endregion

        #region Validation Methods - Các method validation

        /// <summary>
        /// Kiểm tra danh mục có tồn tại trong database không
        /// </summary>
        /// <param name="categoryId">ID của danh mục cần kiểm tra</param>
        /// <returns>true nếu danh mục tồn tại, false nếu không</returns>
        Task<bool> ValidateCategoryExistsAsync(Guid categoryId);

        /// <summary>
        /// Kiểm tra tên món ăn đã tồn tại chưa
        /// </summary>
        /// <param name="name">Tên món ăn cần kiểm tra</param>
        /// <param name="excludeId">ID món ăn cần loại trừ (dùng cho update)</param>
        /// <returns>true nếu tên đã tồn tại, false nếu chưa</returns>
        Task<bool> ValidateMenuItemNameExistsAsync(string name, Guid? excludeId = null);

        /// <summary>
        /// Kiểm tra món ăn có thể xóa được không (không có trong đơn hàng)
        /// </summary>
        /// <param name="id">ID của món ăn cần kiểm tra</param>
        /// <returns>true nếu có thể xóa, false nếu không thể xóa</returns>
        Task<bool> ValidateMenuItemCanBeDeletedAsync(Guid id);

        #endregion
    }
}
