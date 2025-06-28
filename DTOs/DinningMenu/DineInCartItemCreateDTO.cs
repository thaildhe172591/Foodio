namespace FoodioAPI.DTOs.DinningMenu
{
    public class DineInCartItemCreateDTO
    {
        public Guid MenuItemId { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }

}
