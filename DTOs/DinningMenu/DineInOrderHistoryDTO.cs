namespace FoodioAPI.DTOs.DinningMenu
{
    public class DineInOrderHistoryDTO
    {
        public Guid OrderId { get; set; }
        public decimal Total { get; set; }
        public string OrderStatus { get; set; } = default!;
        public DateTime CreatedAt { get; set; }

        public List<DineInOrderItemStatusDTO> Items { get; set; } = new();
    }
}
