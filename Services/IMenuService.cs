using FoodioAPI.DTOs.Menu;

namespace FoodioAPI.Services
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuItemDto>> GetMenuItemsAsync(FilterMenuItemDto filter);
    }
}
