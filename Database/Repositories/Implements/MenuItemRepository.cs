using FoodioAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodioAPI.Database.Repositories.Implements
{
    public class MenuItemRepository : BaseRepository<MenuItem>, IMenuItemRepository
    {
        private readonly ApplicationDbContext _context;
        public MenuItemRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuItem>> GetByCategoryAsync(Guid categoryId)
        {
            return await _context.MenuItems
                .Include(m => m.Category)
                .Where(m => m.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<MenuItem> GetByIdAsync(Guid id)
        {
            return await _context.MenuItems
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<MenuItem>> GetByAvailabilityAsync(bool isAvailable)
        {
            return await _context.MenuItems
                .Include(m => m.Category)
                .Where(m => m.IsAvailable == isAvailable)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> SearchByNameAsync(string name)
        {
            return await _context.MenuItems
                .Include(m => m.Category)
                .Where(m => m.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
        {
            var query = _context.MenuItems.Where(m => m.Name.ToLower() == name.ToLower());

            if (excludeId.HasValue)
                query = query.Where(m => m.Id != excludeId.Value);

            return await query.AnyAsync();
        }
        public async Task<List<MenuItem>> GetAllWithCategoryAsync()
        {
            return await _context.MenuItems.Include(m => m.Category).ToListAsync();
        }

        public async Task<bool> HasOrdersAsync(Guid menuItemId)
        {
            return await _context.OrderItems.AnyAsync(oi => oi.MenuItemId == menuItemId);
        }
    }
}