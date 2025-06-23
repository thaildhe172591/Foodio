namespace FoodioAPI.DTOs.Cart
{
    public class UpdateCartItemDto
    {
        public Guid CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}