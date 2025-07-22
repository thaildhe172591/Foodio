using FoodioAPI.DTOs;
using FoodioAPI.DTOs.Order;

namespace FoodioAPI.Services
{
    public interface IOrderV2Service
    {
        Task<Response> CreateOrderAsync(CreateOrderRequestDTO request, string userId);
    }
} 