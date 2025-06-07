namespace FoodioAPI.DTOs.UserDtos
{
    public class RevenueByUserDto
    {
        public string UserName { get; set; } = default!;
        public decimal TotalRevenue { get; set; }
        public int OrderCount { get; set; }
    }
}
