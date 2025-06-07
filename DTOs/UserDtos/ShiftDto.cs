namespace FoodioAPI.DTOs.UserDtos
{
    public class ShiftDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = default!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Role { get; set; } = default!;
    }
}
