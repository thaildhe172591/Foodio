using FoodioAPI.DTOs.Cart;
using FoodioAPI.DTOs.Menu;
using FoodioAPI.DTOs.Order;

namespace FoodioAPI.Services;

public interface ICartService
{
    Task AddToCartAsync(string userId, AddToCartDto dto);
    Task<bool> AddMenuItemToCartAsync(string userId, AddMenuItemToCartDto dto);
    Task<List<CartItemDto>> GetCartItemsAsync(string userId);
    Task<IEnumerable<CartItemDto>> GetCartAsync(string userId);
    Task<bool> SubmitDeliveryInfoAsync(string userId, DeliveryInfoDto dto);
    Task<bool> ConfirmOrderAsync(string userId, ConfirmOrderDto dto);
}