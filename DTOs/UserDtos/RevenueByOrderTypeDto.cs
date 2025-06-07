namespace FoodioAPI.DTOs.UserDtos
{
    public class RevenueByOrderTypeDto
    {
        public string OrderTypeName { get; set; } = default!;
        public decimal TotalRevenue { get; set; }
        public int OrderCount { get; set; }
    }
}
