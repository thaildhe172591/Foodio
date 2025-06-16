using FoodioAPI.DTOs.Order;
using System.Threading.Tasks;

namespace FoodioAPI.Services;

public interface IOrderService
{
    Task<List<OrderStatusDto>> GetOrderStatusesAsync(string userId);
}
