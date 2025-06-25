namespace FoodioAPI.DTOs.Menu;
public class MenuItemv2Dto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string Category { get; set; } = default!;
    public Guid CategoryId { get; set; } = Guid.Empty;
    public bool IsAvailable { get; set; } = true;
}
