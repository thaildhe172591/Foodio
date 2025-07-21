namespace FoodioAPI.DTOs.DinningMenu
{
    public class DineInOrderItemStatusDTO
    {
        public Guid MenuItemId { get; set; }
        public string Name { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Note { get; set; }
        public string LatestStatus { get; set; } = default!;
        public DateTime StatusChangedAt { get; set; }
    }
}
