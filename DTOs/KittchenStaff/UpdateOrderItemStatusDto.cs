namespace FoodioAPI.DTOs.KittchenStaff
{
    public class UpdateOrderItemStatusDto
    {
        public Guid OrderItemId { get; set; }
        public string NewStatusCode { get; set; } = default!;
    }
}
