using FoodioAPI.Database;
using FoodioAPI.Database.Repositories;
using FoodioAPI.DTOs.Menu;
using FoodioAPI.DTOs.UserDtos;
using FoodioAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodioAPI.Services.Implements
{
    /// <summary>
    /// Service quản lý món ăn trong menu
    /// Chứa business logic cho việc CRUD và quản lý trạng thái món ăn
    /// </summary>
    public class MenuService : IMenuService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMenuItemRepository _menuItemRepository;

        public MenuService(ApplicationDbContext context, IMenuItemRepository menuItemRepository)
        {
            _context = context;
            _menuItemRepository = menuItemRepository;
        }

        #region Read Operations - Lấy thông tin món ăn

        /// <summary>
        /// Lấy danh sách món ăn với bộ lọc tùy chỉnh
        /// Hỗ trợ tìm kiếm theo tên và lọc theo danh mục
        /// </summary>
        /// <param name="filter">Bộ lọc cho việc tìm kiếm và phân loại món ăn</param>
        /// <returns>Danh sách món ăn khớp với bộ lọc</returns>
        public async Task<IEnumerable<MenuItemv2Dto>> GetMenuItemsAsync(FilterMenuItemDto filter)
        {
            var query = _context.MenuItems
                .Include(mi => mi.Category)
                .AsQueryable();

            // Áp dụng bộ lọc tìm kiếm theo tên
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(mi => mi.Name.ToLower().Contains(filter.Search.ToLower()));
            }

            // Áp dụng bộ lọc theo danh mục
            if (filter.CategoryId.HasValue)
            {
                query = query.Where(mi => mi.CategoryId == filter.CategoryId.Value);
            }

            return await query.Select(mi => new MenuItemv2Dto
            {
                Id = mi.Id,
                Name = mi.Name,
                Description = mi.Description,
                Price = mi.Price,
                ImageUrl = mi.ImageUrl,
                Category = mi.Category.Name
            }).ToListAsync();
        }

        /// <summary>
        /// Tìm kiếm và lọc món ăn với nhiều tham số (tên, danh mục, trạng thái, giá, phân trang, sắp xếp)
        /// </summary>
        /// <param name="searchDto">Tham số tìm kiếm và lọc</param>
        /// <returns>Kết quả tìm kiếm với phân trang và thống kê</returns>
        public async Task<MenuItemSearchResultDto> SearchMenuItemsWithPaginationAsync(MenuItemSearchDto searchDto)
        {
            // Validation
            if (!searchDto.IsPriceRangeValid())
            {
                throw new ArgumentException("Giá tối thiểu không được lớn hơn giá tối đa");
            }

            // Xây dựng query cơ bản
            var query = _context.MenuItems
                .Include(m => m.Category)
                .AsQueryable();

            // Áp dụng các bộ lọc
            query = ApplySearchFilters(query, searchDto);

            // Tính tổng số món ăn thỏa mãn điều kiện (trước khi phân trang)
            var totalCount = await query.CountAsync();

            // Áp dụng sắp xếp
            query = ApplySorting(query, searchDto);

            // Áp dụng phân trang
            var skip = (searchDto.Page - 1) * searchDto.PageSize;
            var menuItems = await query
                .Skip(skip)
                .Take(searchDto.PageSize)
                .ToListAsync();

            // Tính toán thống kê
            var statistics = await CalculateSearchStatistics(searchDto);

            // Tạo kết quả
            var result = new MenuItemSearchResultDto
            {
                Items = menuItems.Select(MapToDto),
                TotalCount = totalCount,
                CurrentPage = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize),
                SearchCriteria = searchDto,
                Statistics = statistics
            };

            result.HasNextPage = result.CurrentPage < result.TotalPages;
            result.HasPreviousPage = result.CurrentPage > 1;

            return result;
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một món ăn theo ID
        /// </summary>
        /// <param name="id">ID của món ăn</param>
        /// <returns>Thông tin chi tiết món ăn, null nếu không tìm thấy</returns>
        public async Task<MenuItemv2Dto?> GetMenuItemByIdAsync(Guid id)
        {
            var menuItem = await _context.MenuItems
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            return menuItem is not null ? MapToDto(menuItem) : null;
        }

        #endregion

        #region Create, Update, Delete Operations - CRUD cơ bản

        /// <summary>
        /// Tạo món ăn mới trong menu
        /// Thực hiện validation trước khi tạo
        /// </summary>
        /// <param name="dto">Thông tin món ăn cần tạo</param>
        /// <returns>Thông tin món ăn đã tạo thành công</returns>
        /// <exception cref="InvalidOperationException">Khi danh mục không tồn tại hoặc tên món ăn đã trùng</exception>
        public async Task<MenuItemv2Dto> CreateMenuItemAsync(CreateMenuItemDto dto)
        {
            // Validation: Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Tên món ăn không được để trống");

            if (dto.Price < 0)
                throw new ArgumentException("Giá món ăn không được âm");

            // Validation: Kiểm tra danh mục tồn tại
            if (!await ValidateCategoryExistsAsync(dto.CategoryId))
                throw new InvalidOperationException("Danh mục không tồn tại");

            // Validation: Kiểm tra tên món ăn không trùng
            if (await ValidateMenuItemNameExistsAsync(dto.Name))
                throw new InvalidOperationException("Tên món ăn đã tồn tại");

            // Tạo entity mới
            var menuItem = new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                Price = dto.Price,
                ImageUrl = dto.ImageUrl?.Trim(),
                CategoryId = dto.CategoryId,
                IsAvailable = dto.IsAvailable
            };

            // Lưu vào database
            await _menuItemRepository.AddAsync(menuItem);
            await _context.SaveChangesAsync();

            return MapToDto(menuItem);
        }

        /// <summary>
        /// Cập nhật thông tin món ăn
        /// Kiểm tra ràng buộc trước khi cập nhật
        /// </summary>
        /// <param name="id">ID của món ăn cần cập nhật</param>
        /// <param name="dto">Thông tin mới của món ăn</param>
        /// <returns>true nếu cập nhật thành công, false nếu không tìm thấy món ăn</returns>
        /// <exception cref="InvalidOperationException">Khi danh mục không tồn tại hoặc tên món ăn đã trùng</exception>
        public async Task<bool> UpdateMenuItemAsync(Guid id, UpdateMenuItemDto dto)
        {
            // Validation: Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Tên món ăn không được để trống");

            if (dto.Price < 0)
                throw new ArgumentException("Giá món ăn không được âm");

            var menuItem = await _menuItemRepository.GetByIdAsync(id);
            if (menuItem is null)
                return false;

            // Validation: Kiểm tra danh mục tồn tại
            if (!await ValidateCategoryExistsAsync(dto.CategoryId))
                throw new InvalidOperationException("Danh mục không tồn tại");

            // Validation: Kiểm tra tên món ăn không trùng (loại trừ món ăn hiện tại)
            if (await ValidateMenuItemNameExistsAsync(dto.Name, id))
                throw new InvalidOperationException("Tên món ăn đã tồn tại");

            // Cập nhật thông tin
            menuItem.Name = dto.Name.Trim();
            menuItem.Description = dto.Description?.Trim();
            menuItem.Price = dto.Price;
            menuItem.ImageUrl = dto.ImageUrl?.Trim();
            menuItem.CategoryId = dto.CategoryId;
            menuItem.IsAvailable = dto.IsAvailable;

            // Lưu thay đổi
            await _menuItemRepository.UpdateAsync(menuItem);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Xóa món ăn khỏi menu
        /// Kiểm tra ràng buộc trước khi xóa
        /// </summary>
        /// <param name="id">ID của món ăn cần xóa</param>
        /// <returns>true nếu xóa thành công, false nếu không tìm thấy món ăn</returns>
        /// <exception cref="InvalidOperationException">Khi không thể xóa món ăn đã có trong đơn hàng</exception>
        public async Task<bool> DeleteMenuItemAsync(Guid id)
        {
            // Validation: Kiểm tra có thể xóa không
            if (!await ValidateMenuItemCanBeDeletedAsync(id))
                throw new InvalidOperationException("Không thể xóa món ăn đã có trong đơn hàng");

            var menuItem = await _menuItemRepository.GetByIdAsync(id);
            if (menuItem is null)
                return false;

            // Xóa món ăn
            await _menuItemRepository.DeleteAsync(menuItem);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Partial Update Operations - Cập nhật từng phần

        /// <summary>
        /// Cập nhật trạng thái còn món/hết món
        /// </summary>
        /// <param name="id">ID của món ăn</param>
        /// <param name="isAvailable">true: còn món, false: hết món</param>
        /// <returns>true nếu cập nhật thành công, false nếu không tìm thấy món ăn</returns>
        public async Task<bool> UpdateAvailabilityAsync(Guid id, bool isAvailable)
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(id);
            if (menuItem is null)
                return false;

            menuItem.IsAvailable = isAvailable;
            await _menuItemRepository.UpdateAsync(menuItem);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Cập nhật giá món ăn
        /// Kiểm tra giá không được âm
        /// </summary>
        /// <param name="id">ID của món ăn</param>
        /// <param name="price">Giá mới của món ăn</param>
        /// <returns>true nếu cập nhật thành công, false nếu không tìm thấy món ăn</returns>
        /// <exception cref="ArgumentException">Khi giá âm</exception>
        public async Task<bool> UpdatePriceAsync(Guid id, decimal price)
        {
            if (price < 0)
                throw new ArgumentException("Giá không được âm");

            var menuItem = await _menuItemRepository.GetByIdAsync(id);
            if (menuItem is null)
                return false;

            menuItem.Price = price;
            await _menuItemRepository.UpdateAsync(menuItem);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Cập nhật hình ảnh món ăn
        /// </summary>
        /// <param name="id">ID của món ăn</param>
        /// <param name="imageUrl">URL hình ảnh mới</param>
        /// <returns>true nếu cập nhật thành công, false nếu không tìm thấy món ăn</returns>
        public async Task<bool> UpdateImageAsync(Guid id, string imageUrl)
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(id);
            if (menuItem is null)
                return false;

            menuItem.ImageUrl = imageUrl;
            await _menuItemRepository.UpdateAsync(menuItem);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Validation Methods - Các method validation

        /// <summary>
        /// Kiểm tra danh mục có tồn tại trong database không
        /// </summary>
        /// <param name="categoryId">ID của danh mục cần kiểm tra</param>
        /// <returns>true nếu danh mục tồn tại, false nếu không</returns>
        public async Task<bool> ValidateCategoryExistsAsync(Guid categoryId)
        {
            return await _context.Categories.AnyAsync(c => c.Id == categoryId);
        }

        /// <summary>
        /// Kiểm tra tên món ăn đã tồn tại chưa
        /// Hỗ trợ loại trừ một món ăn cụ thể (dùng cho update)
        /// </summary>
        /// <param name="name">Tên món ăn cần kiểm tra</param>
        /// <param name="excludeId">ID món ăn cần loại trừ (dùng cho update)</param>
        /// <returns>true nếu tên đã tồn tại, false nếu chưa</returns>
        public async Task<bool> ValidateMenuItemNameExistsAsync(string name, Guid? excludeId = null)
        {
            return await _menuItemRepository.ExistsByNameAsync(name, excludeId);
        }

        /// <summary>
        /// Kiểm tra món ăn có thể xóa được không (không có trong đơn hàng)
        /// </summary>
        /// <param name="id">ID của món ăn cần kiểm tra</param>
        /// <returns>true nếu có thể xóa, false nếu không thể xóa</returns>
        public async Task<bool> ValidateMenuItemCanBeDeletedAsync(Guid id)
        {
            return !await _menuItemRepository.HasOrdersAsync(id);
        }

        #endregion

        #region Private Helper Methods - Các method hỗ trợ

        /// <summary>
        /// Chuyển đổi MenuItem entity thành MenuItemv2Dto
        /// </summary>
        /// <param name="menuItem">MenuItem entity</param>
        /// <returns>MenuItemv2Dto</returns>
        private static MenuItemv2Dto MapToDto(MenuItem menuItem)
        {
            return new MenuItemv2Dto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                ImageUrl = menuItem.ImageUrl,
                CategoryId = menuItem.CategoryId,
                IsAvailable = menuItem.IsAvailable
            };
        }

        /// <summary>
        /// Áp dụng các bộ lọc tìm kiếm vào query
        /// </summary>
        /// <param name="query">Query cơ bản</param>
        /// <param name="searchDto">Tham số tìm kiếm</param>
        /// <returns>Query đã được áp dụng bộ lọc</returns>
        private IQueryable<MenuItem> ApplySearchFilters(IQueryable<MenuItem> query, MenuItemSearchDto searchDto)
        {
            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                query = query.Where(m => m.Name.ToLower().Contains(searchDto.SearchTerm.ToLower()));
            }

            // Lọc theo danh mục
            if (searchDto.CategoryId.HasValue)
            {
                query = query.Where(m => m.CategoryId == searchDto.CategoryId.Value);
            }

            // Lọc theo trạng thái
            if (searchDto.IsAvailable.HasValue)
            {
                query = query.Where(m => m.IsAvailable == searchDto.IsAvailable.Value);
            }

            // Lọc theo giá tối thiểu
            if (searchDto.MinPrice.HasValue)
            {
                query = query.Where(m => m.Price >= searchDto.MinPrice.Value);
            }

            // Lọc theo giá tối đa
            if (searchDto.MaxPrice.HasValue)
            {
                query = query.Where(m => m.Price <= searchDto.MaxPrice.Value);
            }

            return query;
        }

        /// <summary>
        /// Áp dụng sắp xếp vào query
        /// </summary>
        /// <param name="query">Query cơ bản</param>
        /// <param name="searchDto">Tham số sắp xếp</param>
        /// <returns>Query đã được sắp xếp</returns>
        private IQueryable<MenuItem> ApplySorting(IQueryable<MenuItem> query, MenuItemSearchDto searchDto)
        {
            var sortBy = searchDto.SortBy?.ToLower() ?? "name";
            var sortOrder = searchDto.SortOrder?.ToLower() ?? "asc";

            switch (sortBy)
            {
                case "name":
                    query = sortOrder == "desc"
                        ? query.OrderByDescending(m => m.Name)
                        : query.OrderBy(m => m.Name);
                    break;
                case "price":
                    query = sortOrder == "desc"
                        ? query.OrderByDescending(m => m.Price)
                        : query.OrderBy(m => m.Price);
                    break;
                case "createddate":
                    query = sortOrder == "desc"
                        ? query.OrderByDescending(m => m.Id) // Sử dụng ID làm proxy cho created date
                        : query.OrderBy(m => m.Id);
                    break;
                default:
                    query = query.OrderBy(m => m.Name);
                    break;
            }

            return query;
        }

        /// <summary>
        /// Tính toán thống kê về kết quả tìm kiếm
        /// </summary>
        /// <param name="searchDto">Tham số tìm kiếm</param>
        /// <returns>Thống kê về kết quả tìm kiếm</returns>
        private async Task<SearchStatistics> CalculateSearchStatistics(MenuItemSearchDto searchDto)
        {
            var query = _context.MenuItems.AsQueryable();

            // Áp dụng các bộ lọc tương tự như tìm kiếm chính
            query = ApplySearchFilters(query, searchDto);

            var statistics = new SearchStatistics
            {
                AvailableCount = await query.CountAsync(m => m.IsAvailable),
                UnavailableCount = await query.CountAsync(m => !m.IsAvailable),
                AveragePrice = await query.AverageAsync(m => m.Price),
                MinPrice = await query.MinAsync(m => m.Price),
                MaxPrice = await query.MaxAsync(m => m.Price)
            };

            return statistics;
        }

        #endregion
    }
}
