using FoodioAPI.DTOs.DinningMenu;

namespace FoodioAPI.Services
{
    public interface IDineInCartService
    {
        Task AddOrUpdateCartItemAsync(Guid tableId, DineInCartItemCreateDTO dto);
        Task<List<DineInCartItemResponseDTO>> GetCartAsync(Guid tableId);
        Task<decimal> GetTotalAsync(Guid tableId);
        Task<Guid> PlaceOrderAsync(Guid tableId);

    }

}
