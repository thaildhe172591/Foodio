using FoodioAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodioAPI.Controllers
{
    [ApiController]
    [Route("api/cashier/order")]
    public class CashierOrderController : ControllerBase
    {
        private readonly ICashierOrderService _orderService;

        public CashierOrderController(ICashierOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPut("confirm/{orderId}")]
        public async Task<IActionResult> ConfirmOrder(Guid orderId)
        {
            await _orderService.ConfirmOrderAsync(orderId);
            return Ok(new { message = "Order confirmed successfully." });
        }
    }
}
