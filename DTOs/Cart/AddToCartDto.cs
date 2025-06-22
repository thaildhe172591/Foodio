namespace FoodioAPI.DTOs.Cart
{
    public class AddToCartDto
    {
        public Guid MenuItemId { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }
}
