namespace FoodioAPI.Services
{
    public interface ICashierOrderService
    {
        Task ConfirmOrderAsync(Guid orderId);
    }
}
