using FoodioAPI.DTOs;
using FoodioAPI.DTOs.Order;

namespace FoodioAPI.Services
{
    public interface IOrderV2Service
    {
        Task<Response?> AddShipOrder(AddShipOrderRequestDTO addShipOrderRequest);
        Task<Response> CreateOrderAsync(CreateOrderRequestDTO request, string userName);
        Task<OrderDetailDTO?> GetOrderDetailAsync(Guid orderId, string userName);
        Task<List<OrderSummaryDTO>> GetOrderSummariesAsync(string userId);
        Task<List<OrderSummaryDTO>> GetOrderSummariesCashAsync(string userId);
        Task<List<DiningTableDTO>> GetDiningTablesAsync();
    }
} 