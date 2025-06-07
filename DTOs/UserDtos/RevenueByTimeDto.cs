namespace FoodioAPI.DTOs.UserDtos
{
    public class RevenueByTimeDto
    {
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }
        public int OrderCount { get; set; }
    }
}
