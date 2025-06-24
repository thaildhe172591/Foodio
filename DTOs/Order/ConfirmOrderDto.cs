namespace FoodioAPI.DTOs.Order;

public class ConfirmOrderDto
{
    public string Type { get; set; }
    public string? ReceiverName { get; set; }
    public string? ReceiverPhone { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? Note { get; set; }
}

