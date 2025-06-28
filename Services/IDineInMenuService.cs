using FoodioAPI.DTOs.DinningMenu;

namespace FoodioAPI.Services
{
    public interface IDineInMenuService
    {
        Task<List<QrMenuCategoryDTO>> GetMenuGroupedByCategoryAsync();
    }

}
