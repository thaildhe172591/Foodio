using FoodioAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FoodioAPI.DTOs.DinningMenu;

namespace FoodioAPI.Controllers
    {
        [ApiController]
        [Route("api/dinein/order")]
        public class DineInOrderController : ControllerBase
        {
            private readonly IDineInOrderService _orderService;

            public DineInOrderController(IDineInOrderService orderService)
            {
                _orderService = orderService;
            }

        [HttpGet("history")]
        public async Task<IActionResult> GetOrderHistory()
        {
            if (!HttpContext.Items.TryGetValue("TableId", out var tableIdObj))
                return Unauthorized("Missing table token.");

            var tableId = (Guid)tableIdObj;

            var result = await _orderService.GetOrderHistoryByTableAsync(tableId);
            return Ok(result);
        }

        [HttpPut("request-payment/{orderId}")]
        public async Task<IActionResult> RequestPayment(Guid orderId)
        {
            if (!HttpContext.Items.TryGetValue("TableId", out var tableIdObj))
                return Unauthorized("Missing table token.");

            var tableId = (Guid)tableIdObj;

            await _orderService.RequestPaymentAsync(tableId, orderId);
            return Ok(new { message = "Payment request submitted successfully." });
        }

        [HttpPost("call-staff/{orderId}")]
        public async Task<IActionResult> CallStaff(Guid orderId)
        {
            if (!HttpContext.Items.TryGetValue("TableId", out var tableIdObj))
                return Unauthorized("Missing table token.");

            var tableId = (Guid)tableIdObj;

            await _orderService.CallStaffAsync(tableId, orderId);
            return Ok(new { message = "Staff request submitted successfully." });
        }

    }
}
