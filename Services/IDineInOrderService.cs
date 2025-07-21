using FoodioAPI.DTOs.DinningMenu;

namespace FoodioAPI.Services
{
    public interface IDineInOrderService
    {
        Task<List<DineInOrderHistoryDTO>> GetOrderHistoryByTableAsync(Guid tableId);
        Task RequestPaymentAsync(Guid tableId, Guid orderId);
        Task CallStaffAsync(Guid tableId, Guid orderId);
    }

}
