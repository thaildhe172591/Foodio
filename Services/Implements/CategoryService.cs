using FoodioAPI.Database;
using FoodioAPI.DTOs.UserDtos;
using FoodioAPI.Entities;
using FoodioAPI.Services;
using Microsoft.EntityFrameworkCore;
using FoodioAPI.DTOs.KitchenStaff;
using System.Net.WebSockets; // Thêm namespace đúng cho OrderWithDetails

namespace FoodioAPI.Services.Implements
{
    /// <summary>
    /// Service quản lý danh mục
    /// Implement business logic cho việc CRUD danh mục
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        //Hiếu
        public async Task<List<OrderWithDetails>> GetOrdersWithStatusPendingAsync()
        {
            var result = await (
          from oi in _context.OrderItems
          join o in _context.Orders on oi.OrderId equals o.Id
          join oish in _context.OrderItemStatusHistories on oi.Id equals oish.OrderItemId
          join ois in _context.OrderItemStatuses on oish.StatusId equals ois.Id
          join mi in _context.MenuItems on oi.MenuItemId equals mi.Id
          join c in _context.Categories on mi.CategoryId equals c.Id
          join u in _context.Users on o.UserId equals u.Id into userJoin
          from u in userJoin.DefaultIfEmpty()
          join t in _context.DiningTables on o.TableId equals t.Id into tableJoin
          from t in tableJoin.DefaultIfEmpty()
          where c.Name.Contains("Món Lạnh") && ois.Code == "PENDING"
          select new OrderWithDetails
          {
              OrderItemId = oish.Id,    
              OrderId = o.Id,
              UserName = u != null ? u.UserName : null,
              MenuItemName = mi.Name,
              CategoryName = c.Name,
              Note = oi.Note,
              Quantity = oi.Quantity,
              TableNumber = t != null ? t.TableNumber : (int?)null,
              ItemStatusCode = ois.Code
          }
      ).ToListAsync();

            return result;
        }
        // Đơn hàng PENDING - Món Nóng
        public async Task<List<OrderWithDetails>> GetPendingHotDishOrdersAsync()
        {
            var result = await (
                from oi in _context.OrderItems
                join o in _context.Orders on oi.OrderId equals o.Id
                join oish in _context.OrderItemStatusHistories on oi.Id equals oish.OrderItemId
                join ois in _context.OrderItemStatuses on oish.StatusId equals ois.Id
                join mi in _context.MenuItems on oi.MenuItemId equals mi.Id
                join c in _context.Categories on mi.CategoryId equals c.Id
                join u in _context.Users on o.UserId equals u.Id into userJoin
                from u in userJoin.DefaultIfEmpty()
                join t in _context.DiningTables on o.TableId equals t.Id into tableJoin
                from t in tableJoin.DefaultIfEmpty()
                where c.Name.Contains("Món Nóng") && ois.Code == "PENDING"
                select new OrderWithDetails
                {
                    OrderItemId = oish.Id,
                    OrderId = o.Id,
                    UserName = u != null ? u.UserName : null,
                    MenuItemName = mi.Name,
                    CategoryName = c.Name,
                    Note = oi.Note,
                    Quantity = oi.Quantity,
                    TableNumber = t != null ? t.TableNumber : (int?)null,
                    ItemStatusCode = ois.Code
                }
            ).ToListAsync();

            return result;
        }
        // Đơn hàng PENDING - Nước Uống
        public async Task<List<OrderWithDetails>> GetPendingDrinksDishOrdersAsync()
        {
            var result = await (
                from oi in _context.OrderItems
                join o in _context.Orders on oi.OrderId equals o.Id
                join oish in _context.OrderItemStatusHistories on oi.Id equals oish.OrderItemId
                join ois in _context.OrderItemStatuses on oish.StatusId equals ois.Id
                join mi in _context.MenuItems on oi.MenuItemId equals mi.Id
                join c in _context.Categories on mi.CategoryId equals c.Id
                join u in _context.Users on o.UserId equals u.Id into userJoin
                from u in userJoin.DefaultIfEmpty()
                join t in _context.DiningTables on o.TableId equals t.Id into tableJoin
                from t in tableJoin.DefaultIfEmpty()
                where c.Name.Contains("Nước") && ois.Code == "PENDING"
                select new OrderWithDetails
                {
                    OrderItemId = oish.Id,
                    OrderId = o.Id,
                    UserName = u != null ? u.UserName : null,
                    MenuItemName = mi.Name,
                    CategoryName = c.Name,
                    Note = oi.Note,
                    Quantity = oi.Quantity,
                    TableNumber = t != null ? t.TableNumber : (int?)null,
                    ItemStatusCode = ois.Code
                }
            ).ToListAsync();

            return result;
        }
        //Danh sách Coooking món lạnh 
        public async Task<List<OrderWithDetails>> GetOrdersWithStatusCookingColdAsync()
        {
            var result = await (
                from oi in _context.OrderItems
                join o in _context.Orders on oi.OrderId equals o.Id
                join oish in _context.OrderItemStatusHistories on oi.Id equals oish.OrderItemId
                join ois in _context.OrderItemStatuses on oish.StatusId equals ois.Id
                join mi in _context.MenuItems on oi.MenuItemId equals mi.Id
                join c in _context.Categories on mi.CategoryId equals c.Id
                join u in _context.Users on o.UserId equals u.Id into userJoin
                from u in userJoin.DefaultIfEmpty()
                join t in _context.DiningTables on o.TableId equals t.Id into tableJoin
                from t in tableJoin.DefaultIfEmpty()
                where c.Name.Contains("Món Lạnh") && ois.Code == "COOKING"
                select new OrderWithDetails
                {
                    OrderItemId = oish.Id,
                    OrderId = o.Id,
                    UserName = u != null ? u.UserName : null,
                    MenuItemName = mi.Name,
                    CategoryName = c.Name,
                    Note = oi.Note,
                    Quantity = oi.Quantity,
                    TableNumber = t != null ? t.TableNumber : (int?)null,
                    ItemStatusCode = ois.Code
                }
            ).ToListAsync();

            return result;
        }
        //Danh sách Coooking món nóng 
        public async Task<List<OrderWithDetails>> GetOrdersWithStatusCookingHotAsync()
        {
            var result = await (
                from oi in _context.OrderItems
                join o in _context.Orders on oi.OrderId equals o.Id
                join oish in _context.OrderItemStatusHistories on oi.Id equals oish.OrderItemId
                join ois in _context.OrderItemStatuses on oish.StatusId equals ois.Id
                join mi in _context.MenuItems on oi.MenuItemId equals mi.Id
                join c in _context.Categories on mi.CategoryId equals c.Id
                join u in _context.Users on o.UserId equals u.Id into userJoin
                from u in userJoin.DefaultIfEmpty()
                join t in _context.DiningTables on o.TableId equals t.Id into tableJoin
                from t in tableJoin.DefaultIfEmpty()
                where c.Name.Contains("Món Nóng") && ois.Code == "COOKING"
                select new OrderWithDetails
                {
                    OrderItemId = oish.Id,
                    OrderId = o.Id,
                   
                    UserName = u != null ? u.UserName : null,
                    MenuItemName = mi.Name,
                    CategoryName = c.Name,
                    Note = oi.Note,
                    Quantity = oi.Quantity,
                    TableNumber = t != null ? t.TableNumber : (int?)null,
                    ItemStatusCode = ois.Code
                }
            ).ToListAsync();

            return result;
        }
        //Danh sách Coooking Drinks
        public async Task<List<OrderWithDetails>> GetOrdersWithStatusCookingDrinksAsync()
        {
            var result = await (
                from oi in _context.OrderItems
                join o in _context.Orders on oi.OrderId equals o.Id
                join oish in _context.OrderItemStatusHistories on oi.Id equals oish.OrderItemId
                join ois in _context.OrderItemStatuses on oish.StatusId equals ois.Id
                join mi in _context.MenuItems on oi.MenuItemId equals mi.Id
                join c in _context.Categories on mi.CategoryId equals c.Id
                join u in _context.Users on o.UserId equals u.Id into userJoin
                from u in userJoin.DefaultIfEmpty()
                join t in _context.DiningTables on o.TableId equals t.Id into tableJoin
                from t in tableJoin.DefaultIfEmpty()
                where c.Name.Contains("Nước") && ois.Code == "COOKING"
                let latestStatus = (
                    from sh in _context.OrderItemStatusHistories
                    where sh.OrderItemId == oi.Id
                    orderby sh.ChangedAt descending
                    select sh
                ).FirstOrDefault()
                where latestStatus != null && latestStatus.StatusId == ois.Id
                select new OrderWithDetails
                {
                    OrderItemId = oish.Id,
                    OrderId = o.Id,
                    UserName = u != null ? u.UserName : null,
                    MenuItemName = mi.Name,
                    CategoryName = c.Name,
                    Note = oi.Note,
                    Quantity = oi.Quantity,
                    TableNumber = t != null ? t.TableNumber : (int?)null,
                    ItemStatusCode = ois.Code
                }
            ).ToListAsync();

            return result;
        }
        public async Task<List<OrderWithDetails>> GetAllOrderReadyToServeAsync()
        {
            var result = await (
                from oi in _context.OrderItems
                join o in _context.Orders on oi.OrderId equals o.Id
                join oish in _context.OrderItemStatusHistories on oi.Id equals oish.OrderItemId
                join ois in _context.OrderItemStatuses on oish.StatusId equals ois.Id
                join mi in _context.MenuItems on oi.MenuItemId equals mi.Id
                join c in _context.Categories on mi.CategoryId equals c.Id
                join u in _context.Users on o.UserId equals u.Id into userJoin
                from u in userJoin.DefaultIfEmpty()
                join t in _context.DiningTables on o.TableId equals t.Id into tableJoin
                from t in tableJoin.DefaultIfEmpty()
                where ois.Code == "READY_TO_SERVE"
                orderby oish.ChangedAt descending
                select new OrderWithDetails
                {
                    OrderId = o.Id,

                    UserName = u != null ? u.UserName : null,
                    MenuItemName = mi.Name,
                    CategoryName = c.Name,
                    Note = oi.Note,
                    Quantity = oi.Quantity,
                    TableNumber = t != null ? t.TableNumber : (int?)null,
                    ItemStatusCode = ois.Code
                }
            ).ToListAsync();

            return result;
        }
        public async Task<bool> UpdateOrderItemStatusAsync(Guid orderItemId, string newStatusCode)
        {
            // 1. Kiểm tra trạng thái mới
            var status = await _context.OrderItemStatuses.FirstOrDefaultAsync(x => x.Code == newStatusCode);
            if (status == null) return false;

            // 2. Update bản ghi trạng thái mới
            var orderItemStatusHistories = _context.OrderItemStatusHistories.FirstOrDefault(x => x.Id == orderItemId);
            if (orderItemStatusHistories != null)
            {
                orderItemStatusHistories.ChangedAt = DateTime.UtcNow;
                orderItemStatusHistories.StatusId = status.Id;
                _context.OrderItemStatusHistories.Update(orderItemStatusHistories);

                OrderItem orderItem = _context.OrderItems.FirstOrDefault(x => x.Id == orderItemStatusHistories.OrderItemId);
                if (orderItem != null)
                {
                    Order order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderItem.OrderId);
                    order.StatusId = Guid.Parse("0c334fab-07e1-4c5e-bca0-7d2fefe4e0bd");
                    _context.Orders.Update(order);
                }
            }

            // 3. Lưu thay đổi
            await _context.SaveChangesAsync();
            return true;
        }

      




        //đến đấy
        #region Read Operations - Lấy thông tin danh mục

        /// <summary>
        /// Lấy tất cả danh mục với thông tin số lượng món ăn
        /// </summary>
        /// <returns>Danh sách tất cả danh mục</returns>
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.MenuItems)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return categories.Select(MapToDto);
        }

        /// <summary>
        /// Tìm kiếm và lọc danh mục với phân trang
        /// </summary>
        /// <param name="searchDto">Tham số tìm kiếm và lọc</param>
        /// <returns>Kết quả tìm kiếm với phân trang và thống kê</returns>
        public async Task<CategorySearchResultDto> SearchCategoriesWithPaginationAsync(CategorySearchDto searchDto)
        {
            // Xây dựng query cơ bản
            var query = _context.Categories
                .Include(c => c.MenuItems)
                .AsQueryable();

            // Áp dụng các bộ lọc
            query = ApplySearchFilters(query, searchDto);

            // Tính tổng số danh mục thỏa mãn điều kiện (trước khi phân trang)
            var totalCount = await query.CountAsync();

            // Áp dụng sắp xếp
            query = ApplySorting(query, searchDto);

            // Áp dụng phân trang
            var skip = (searchDto.Page - 1) * searchDto.PageSize;
            var categories = await query
                .Skip(skip)
                .Take(searchDto.PageSize)
                .ToListAsync();

            // Tính toán thống kê
            var statistics = await CalculateSearchStatistics(searchDto);

            // Tạo kết quả
            var result = new CategorySearchResultDto
            {
                Items = categories.Select(MapToDto),
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
        /// Lấy thông tin chi tiết của một danh mục theo ID
        /// </summary>
        /// <param name="id">ID của danh mục</param>
        /// <returns>Thông tin chi tiết danh mục, null nếu không tìm thấy</returns>
        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
        {
            var category = await _context.Categories
                .Include(c => c.MenuItems)
                .FirstOrDefaultAsync(c => c.Id == id);

            return category is not null ? MapToDto(category) : null;
        }

        /// <summary>
        /// Tìm kiếm danh mục theo tên
        /// </summary>
        /// <param name="name">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách danh mục khớp với từ khóa</returns>
        /// <exception cref="ArgumentException">Khi từ khóa tìm kiếm rỗng</exception>
        public async Task<IEnumerable<CategoryDto>> SearchCategoriesAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Từ khóa tìm kiếm không được để trống");

            var categories = await _context.Categories
                .Include(c => c.MenuItems)
                .Where(c => c.Name.ToLower().Contains(name.ToLower()))
                .OrderBy(c => c.Name)
                .ToListAsync();

            return categories.Select(MapToDto);
        }

        #endregion

        #region Create, Update, Delete Operations - CRUD cơ bản

        /// <summary>
        /// Tạo danh mục mới
        /// </summary>
        /// <param name="dto">Thông tin danh mục cần tạo</param>
        /// <returns>Thông tin danh mục đã tạo thành công</returns>
        /// <exception cref="InvalidOperationException">Khi tên danh mục đã trùng</exception>
        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            // Validation: Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Tên danh mục không được để trống");

            // Validation: Kiểm tra tên danh mục không trùng
            if (await ValidateCategoryNameExistsAsync(dto.Name))
                throw new InvalidOperationException("Tên danh mục đã tồn tại");

            // Tạo entity mới
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name.Trim()
            };

            // Lưu vào database
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return MapToDto(category);
        }

        /// <summary>
        /// Cập nhật thông tin danh mục
        /// </summary>
        /// <param name="id">ID của danh mục cần cập nhật</param>
        /// <param name="dto">Thông tin mới của danh mục</param>
        /// <returns>true nếu cập nhật thành công, false nếu không tìm thấy danh mục</returns>
        /// <exception cref="InvalidOperationException">Khi tên danh mục đã trùng</exception>
        public async Task<bool> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto)
        {
            // Validation: Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Tên danh mục không được để trống");

            var category = await _context.Categories.FindAsync(id);
            if (category is null)
                return false;

            // Validation: Kiểm tra tên danh mục không trùng (loại trừ danh mục hiện tại)
            if (await ValidateCategoryNameExistsAsync(dto.Name, id))
                throw new InvalidOperationException("Tên danh mục đã tồn tại");

            // Cập nhật thông tin
            category.Name = dto.Name.Trim();

            // Lưu thay đổi
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Xóa danh mục
        /// </summary>
        /// <param name="id">ID của danh mục cần xóa</param>
        /// <returns>true nếu xóa thành công, false nếu không tìm thấy danh mục</returns>
        /// <exception cref="InvalidOperationException">Khi không thể xóa danh mục đã có món ăn</exception>
        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await _context.Categories
                .Include(c => c.MenuItems)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category is null)
                return false;

            // Validation: Kiểm tra danh mục có thể xóa được không
            if (!await ValidateCategoryCanBeDeletedAsync(id))
                throw new InvalidOperationException("Không thể xóa danh mục đã có món ăn");

            // Xóa danh mục
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Validation Methods - Các method validation

        /// <summary>
        /// Kiểm tra tên danh mục đã tồn tại chưa
        /// </summary>
        /// <param name="name">Tên danh mục cần kiểm tra</param>
        /// <param name="excludeId">ID danh mục cần loại trừ (dùng cho update)</param>
        /// <returns>true nếu tên đã tồn tại, false nếu chưa</returns>
        public async Task<bool> ValidateCategoryNameExistsAsync(string name, Guid? excludeId = null)
        {
            var query = _context.Categories.Where(c => c.Name.ToLower() == name.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        /// <summary>
        /// Kiểm tra danh mục có thể xóa được không (không có món ăn)
        /// </summary>
        /// <param name="id">ID của danh mục cần kiểm tra</param>
        /// <returns>true nếu có thể xóa, false nếu không thể xóa</returns>
        public async Task<bool> ValidateCategoryCanBeDeletedAsync(Guid id)
        {
            return !await _context.Categories
                .Where(c => c.Id == id)
                .AnyAsync(c => c.MenuItems.Any());
        }

        #endregion

        #region Private Helper Methods - Các method hỗ trợ

        /// <summary>
        /// Chuyển đổi Category entity thành CategoryDto
        /// </summary>
        /// <param name="category">Category entity</param>
        /// <returns>CategoryDto</returns>
        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                MenuItemCount = category.MenuItems?.Count ?? 0
            };
        }

        /// <summary>
        /// Áp dụng các bộ lọc tìm kiếm vào query
        /// </summary>
        /// <param name="query">Query cơ bản</param>
        /// <param name="searchDto">Tham số tìm kiếm</param>
        /// <returns>Query đã được áp dụng bộ lọc</returns>
        private static IQueryable<Category> ApplySearchFilters(IQueryable<Category> query, CategorySearchDto searchDto)
        {
            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                query = query.Where(c => c.Name.ToLower().Contains(searchDto.SearchTerm.ToLower()));
            }

            return query;
        }

        /// <summary>
        /// Áp dụng sắp xếp vào query
        /// </summary>
        /// <param name="query">Query cơ bản</param>
        /// <param name="searchDto">Tham số sắp xếp</param>
        /// <returns>Query đã được sắp xếp</returns>
        private static IQueryable<Category> ApplySorting(IQueryable<Category> query, CategorySearchDto searchDto)
        {
            var sortBy = searchDto.SortBy?.ToLower() ?? "name";
            var sortOrder = searchDto.SortOrder?.ToLower() ?? "asc";

            switch (sortBy)
            {
                case "name":
                    query = sortOrder == "desc"
                        ? query.OrderByDescending(c => c.Name)
                        : query.OrderBy(c => c.Name);
                    break;
                case "menuitemcount":
                    query = sortOrder == "desc"
                        ? query.OrderByDescending(c => c.MenuItems.Count)
                        : query.OrderBy(c => c.MenuItems.Count);
                    break;
                default:
                    query = query.OrderBy(c => c.Name);
                    break;
            }

            return query;
        }

        /// <summary>
        /// Tính toán thống kê về kết quả tìm kiếm
        /// </summary>
        /// <param name="searchDto">Tham số tìm kiếm</param>
        /// <returns>Thống kê về kết quả tìm kiếm</returns>
        private async Task<CategoryStatistics> CalculateSearchStatistics(CategorySearchDto searchDto)
        {
            var query = _context.Categories
                .Include(c => c.MenuItems)
                .AsQueryable();

            // Áp dụng các bộ lọc tương tự như tìm kiếm chính
            query = ApplySearchFilters(query, searchDto);

            var categories = await query.ToListAsync();

            var statistics = new CategoryStatistics
            {
                TotalCategories = categories.Count,
                CategoriesWithItems = categories.Count(c => c.MenuItems.Any()),
                EmptyCategories = categories.Count(c => !c.MenuItems.Any()),
                TotalMenuItems = categories.Sum(c => c.MenuItems.Count),
                AverageItemsPerCategory = categories.Any() ? (double)categories.Sum(c => c.MenuItems.Count) / categories.Count : 0
            };

            return statistics;
        }

        #endregion
    }
}