using FoodioAPI.Database;
using FoodioAPI.DTOs.Menu;
using Microsoft.EntityFrameworkCore;

namespace FoodioAPI.Services.Implements
{
    public class MenuService : IMenuService
    {
        private readonly ApplicationDbContext _context;

        public MenuService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuItemDto>> GetMenuItemsAsync(FilterMenuItemDto filter)
        {
            var query = _context.MenuItems
                .Include(mi => mi.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(mi => mi.Name.ToLower().Contains(filter.Search.ToLower()));
            }

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(mi => mi.CategoryId == filter.CategoryId.Value);
            }

            return await query.Select(mi => new MenuItemDto
            {
                Id = mi.Id,
                Name = mi.Name,
                Description = mi.Description,
                Price = mi.Price,
                ImageUrl = mi.ImageUrl,
                Category = mi.Category.Name
            }).ToListAsync();
        }
    }
}
