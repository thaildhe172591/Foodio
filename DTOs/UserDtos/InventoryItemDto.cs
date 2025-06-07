namespace FoodioAPI.DTOs.UserDtos
{
    public class InventoryItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public int Quantity { get; set; }
        public string Unit { get; set; } = default!;
        public decimal Price { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
