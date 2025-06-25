using FoodioAPI.DTOs.Cart;
using FoodioAPI.DTOs.Menu;
using FoodioAPI.DTOs.Order;
using FoodioAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        private string GetCurrentUserId()
        {
            //return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            return "044f88e2-0944-4fa0-a27f-7a368a55b35c";

        }
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            string userId = GetCurrentUserId();  // lấy từ bảng Users
            await _cartService.AddToCartAsync(userId, dto);
            return Ok(new { message = "Thêm vào giỏ hàng thành công!" });
        }
        
        [HttpPost("add-menu-item")]
        public async Task<IActionResult> AddMenuItemToCart([FromBody] AddMenuItemToCartDto dto)
        {
            var userId = GetCurrentUserId(); 
            if (userId == null) return Unauthorized();

            var success = await _cartService.AddMenuItemToCartAsync(userId, dto);
            if (!success)
                return BadRequest("Không thể thêm món vào giỏ hàng.");

            return Ok(new { message = "Thêm món thành công!" });
        }
        
        [HttpGet("my-cart")]
        public async Task<IActionResult> GetMyCart()
        {
            var userId = GetCurrentUserId(); 
            var cartItems = await _cartService.GetCartItemsAsync(userId);
            return Ok(cartItems);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetCurrentUserId(); 
            var items = await _cartService.GetCartAsync(userId);
            return Ok(items);
        }

        [HttpPost("delivery-info")]
        public async Task<IActionResult> SubmitDeliveryInfo([FromBody] DeliveryInfoDto dto)
        {
            var userId = GetCurrentUserId();
            var success = await _cartService.SubmitDeliveryInfoAsync(userId, dto);
            if (!success) return BadRequest(new { message = "Không thể lưu thông tin giao hàng." });

            return Ok(new { message = "Lưu thông tin giao hàng thành công!" });
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmOrder([FromBody] ConfirmOrderDto dto)
        {
            var userId = GetCurrentUserId(); 
            var success = await _cartService.ConfirmOrderAsync(userId, dto);
            return success ? Ok(new { message = "Xác nhận đơn hàng thành công!" })
                           : BadRequest(new { message = "Không thể xác nhận đơn hàng." });
        }

        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(Guid cartItemId)
        {
            var userId = GetCurrentUserId();

            var result = await _cartService.RemoveCartItemAsync(userId, cartItemId);
            if (!result)
                return NotFound(new { message = "Không tìm thấy món để xoá." });

            return Ok(new { message = "Đã xoá món khỏi giỏ hàng." });
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _cartService.UpdateCartItemQuantityAsync(userId, dto);

            if (!result)
                return NotFound(new { message = "Không tìm thấy món hoặc giỏ hàng." });

            return Ok(new { message = "Cập nhật số lượng thành công." });
        }
    }
}
