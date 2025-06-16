namespace FoodioAPI.DTOs.Cart;

public class DeliveryInfoDto
{
    public string Type { get; set; } = "delivery"; // hoặc "takeout"
    public string? ReceiverName { get; set; }
    public string? ReceiverPhone { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? Note { get; set; }
}