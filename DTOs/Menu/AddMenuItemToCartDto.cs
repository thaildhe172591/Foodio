namespace FoodioAPI.DTOs.Menu
{
    public class AddMenuItemToCartDto
    {
        public Guid MenuItemId { get; set; }
        public int Quantity { get; set; } = 1;
        public string? Note { get; set; }
    }
}
