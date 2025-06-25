namespace FoodioAPI.DTOs.UserDtos
{
    public class MenuItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsAvailable { get; set; }
    }
}
