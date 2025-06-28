using FoodioAPI.DTOs.DinningMenu;
using FoodioAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodioAPI.Controllers
{
    [ApiController]
    [Route("api/dinein/cart")]
    public class DineInCartController : ControllerBase
    {
        private readonly IDineInCartService _cartService;

        public DineInCartController(IDineInCartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] DineInCartItemCreateDTO dto)
        {
            if (!HttpContext.Items.TryGetValue("TableId", out var tableIdObj))
                return Unauthorized("Missing table token.");

            var tableId = (Guid)tableIdObj;

            await _cartService.AddOrUpdateCartItemAsync(tableId, dto);

            return Ok(new { message = "Item added to cart." });
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            if (!HttpContext.Items.TryGetValue("TableId", out var tableIdObj))
                return Unauthorized("Missing table token.");

            var tableId = (Guid)tableIdObj;

            var items = await _cartService.GetCartAsync(tableId);
            var total = await _cartService.GetTotalAsync(tableId);

            return Ok(new
            {
                items,
                total
            });
        }
    }

}
