using FoodioAPI.Entities;

namespace FoodioAPI.Database.Repositories
{
    public interface IMenuItemRepository : IBaseRepository<MenuItem>
    {
        Task<IEnumerable<MenuItem>> GetByCategoryAsync(Guid categoryId);
        Task<IEnumerable<MenuItem>> GetByAvailabilityAsync(bool isAvailable);
        Task<IEnumerable<MenuItem>> SearchByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
        Task<bool> HasOrdersAsync(Guid menuItemId);
    }
} 