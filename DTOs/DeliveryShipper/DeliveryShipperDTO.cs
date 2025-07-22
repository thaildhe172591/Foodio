namespace FoodioAPI.DTOs.DeliveryShipper
{
    public class DeliveryShipperDTO
    {   public Guid DeliveryId { get; set; }
        public string ReceiverName { get; set; }
        public string MenuItemName { get; set; }
        public string ReceiverPhone { get; set; }
        public string DeliveryAddress { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Code { get; set; }
        public decimal Fee { get; set; }  // Added Fee

        public string DeliveryStatusName { get; set; }
        public string OrderUserName { get; set; }
        public string RoleName { get; set; }
    }
}
