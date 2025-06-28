namespace FoodioAPI.DTOs.DinningMenu
{
    public class QrMenuCategoryDTO
    {
        public string Category { get; set; } = null!;
        public List<QrMenuItemDTO> Items { get; set; } = new();
    }
}
