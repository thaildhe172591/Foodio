namespace FoodioAPI.DTOs.Order
{
    public class DiningTableDTO
    {
        public Guid Id { get; set; }
        public int TableNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsOccupied { get; set; }
        public bool HasActiveOrder { get; set; }
        public DateTime? LastOrderTime { get; set; }
        public int ActiveOrderCount { get; set; }
    }
} 