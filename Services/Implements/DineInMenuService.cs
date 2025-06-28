using FoodioAPI.Database.Repositories;
using FoodioAPI.DTOs.DinningMenu;

namespace FoodioAPI.Services.Implements
{
    public class DineInMenuService : IDineInMenuService
    {
        private readonly IMenuItemRepository _menuItemRepo;

        public DineInMenuService(IMenuItemRepository menuItemRepo)
        {
            _menuItemRepo = menuItemRepo;
        }

        public async Task<List<QrMenuCategoryDTO>> GetMenuGroupedByCategoryAsync()
        {
            var allItems = await _menuItemRepo.GetAllWithCategoryAsync();

            var grouped = allItems
                .GroupBy(m => m.Category.Name)
                .Select(group => new QrMenuCategoryDTO
                {
                    Category = group.Key,
                    Items = group.Select(m => new QrMenuItemDTO
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Description = m.Description,
                        Price = m.Price,
                        ImageUrl = m.ImageUrl,
                        IsAvailable = m.IsAvailable
                    }).ToList()
                })
                .ToList();

            return grouped;
        }
    }

}
