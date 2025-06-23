namespace FoodioAPI.DTOs.Cart
{

    public class CartItemDto
    {
        public Guid MenuItemId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = default!;
        public int Quantity { get; set; }
    }

}
