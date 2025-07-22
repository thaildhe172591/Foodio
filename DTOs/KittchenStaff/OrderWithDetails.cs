namespace FoodioAPI.DTOs.KitchenStaff
{
    public class OrderWithDetails
    {
        public Guid OrderItemId { get; set; }    
        public Guid OrderId { get; set; }
        public string? UserName { get; set; }
        public string MenuItemName { get; set; } = default!;
        public string CategoryName { get; set; } = default!;
        public string? Note { get; set; }
        public int Quantity { get; set; }
        public int? TableNumber { get; set; }
        public string ItemStatusCode { get; set; } = default!;
    }
}
