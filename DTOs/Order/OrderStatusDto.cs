namespace FoodioAPI.DTOs.Order;

public class OrderStatusDto
{
    public Guid OrderId { get; set; }
    public string Status { get; set; }
    public DateTime UpdatedAt { get; set; }
}
