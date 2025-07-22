using FoodioAPI.Services;
using Microsoft.AspNetCore.Mvc;
using FoodioAPI.DTOs.KitchenStaff;
using FoodioAPI.DTOs.KittchenStaff;

namespace FoodioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KitchenStaffController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public KitchenStaffController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Món Lạnh
        [HttpGet("orders/pending")]
        public async Task<IActionResult> GetPendingOrders()
        {
            var orders = await _categoryService.GetOrdersWithStatusPendingAsync();
            if (orders == null || !orders.Any())
                return NotFound("No pending orders found.");
            return Ok(orders);
        }

        // Món Nóng
        [HttpGet("orders/pending/hot")]
        public async Task<IActionResult> GetPendingHotDishOrders()
        {
            var orders = await _categoryService.GetPendingHotDishOrdersAsync();
            if (orders == null || !orders.Any())
                return NotFound("No pending hot dish orders found.");
            return Ok(orders);
        }

        // Nước Uống
        [HttpGet("orders/pending/drinks")]
        public async Task<IActionResult> GetPendingDrinksDishOrders()
        {
            var orders = await _categoryService.GetPendingDrinksDishOrdersAsync();
            if (orders == null || !orders.Any())
                return NotFound("No pending drinks orders found.");
            return Ok(orders);
        }

        // Món Lạnh đang nấu
        [HttpGet("orders/cooking/cold")]
        public async Task<IActionResult> GetOrdersWithStatusCookingCold()
        {
            var orders = await _categoryService.GetOrdersWithStatusCookingColdAsync();
            if (orders == null || !orders.Any())
                return NotFound("No cold dish is being cooked.");
            return Ok(orders);
        }

        // Món Nóng đang nấu
        [HttpGet("orders/cooking/hot")]
        public async Task<IActionResult> GetOrdersWithStatusCookingHot()
        {
            var orders = await _categoryService.GetOrdersWithStatusCookingHotAsync();
            if (orders == null || !orders.Any())
                return NotFound("No hot dish is being cooked.");
            return Ok(orders);
        }

        // Nước Uống đang pha
        [HttpGet("orders/cooking/drinks")]
        public async Task<IActionResult> GetOrdersWithStatusCookingDrinks()
        {
            var orders = await _categoryService.GetOrdersWithStatusCookingDrinksAsync();
            if (orders == null || !orders.Any())
                return NotFound("No drink is being cooked.");
            return Ok(orders);
        }

        // Món sẵn sàng phục vụ
        [HttpGet("ready-to-serve-orders")]
        public async Task<IActionResult> GetAllOrderReadyToServe()
        {
            var orders = await _categoryService.GetAllOrderReadyToServeAsync();
            return Ok(orders);
        }
       
        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateOrderItemStatus([FromBody] UpdateOrderItemStatusDto dto)
        {
            if (dto == null || dto.OrderItemId == Guid.Empty || string.IsNullOrWhiteSpace(dto.NewStatusCode))
                return BadRequest();

            var result = await _categoryService.UpdateOrderItemStatusAsync(dto.OrderItemId, dto.NewStatusCode);
            if (result)
                return Ok();
            return BadRequest();
        }
    }
    // Cập nhật trạng thái order item (Bắt đầu nấu...)


}
