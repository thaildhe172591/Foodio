namespace FoodioAPI.DTOs.DinningMenu
{
    public class DineInCartItemResponseDTO
    {
        public Guid MenuItemId { get; set; }
        public string Name { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total => UnitPrice * Quantity;
        public string? Note { get; set; }
    }

}
