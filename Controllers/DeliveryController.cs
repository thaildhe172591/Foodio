using FoodioAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodioAPI.Controllers
{
    [Route("api/shipper/deliveries")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryShipperService _deliveryShipperService;

        public DeliveryController(IDeliveryShipperService deliveryShipperService)
        {
            _deliveryShipperService = deliveryShipperService;
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var userName = User.Identity.Name;
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

            return Ok(new { userName, email, roles });
        }

        [Authorize]
        [HttpGet("list")]
        public async Task<IActionResult> GetDeliveriesByCurrentShipper()
        {
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized("Không xác định được shipper!");

            var deliveries = await _deliveryShipperService.GetDeliveriesByShipperAsync(userName);

            if (deliveries == null || deliveries.Count == 0)
            {
                return Ok(new { message = "Không có đơn hàng cho shipper này", deliveries = new List<object>() });
            }

            return Ok(deliveries);
        }

        [Authorize]
        [HttpGet("delivered")]
        public async Task<IActionResult> GetAllHaveDeliveriedByShipper()
        {
            var userName = User.Identity.Name;

            if (string.IsNullOrEmpty(userName))
                return Unauthorized("Không xác định được shipper!");

            // Gọi service để lấy các đơn hàng đã giao
            var deliveries = await _deliveryShipperService.GetAllHaveDeliveriedByShipperAsync(userName);

            if (deliveries == null || deliveries.Count == 0)
            {
                return Ok(new { deliveries = deliveries });
            }

            return Ok(deliveries);
        }

        [Authorize]
        [HttpPut("update-status/{deliveryId}")]
        public async Task<IActionResult> UpdateDeliveryStatus(Guid deliveryId, [FromQuery] string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest("Status is required.");
            }

            var result = await _deliveryShipperService.UpdateDeliveryStatusAsync(deliveryId, status);

            if (!result)
                return BadRequest("Cập nhật trạng thái thất bại. Vui lòng kiểm tra lại.");

            if (status.Equals("ON_THE_WAY"))
            {
                return Ok(new { message = $"Trạng thái giao hàng đã được cập nhật thành: Đang Giao" });
            }

            return Ok(new { message = $"Trạng thái giao hàng đã được cập nhật thành '{status}'." });
        }

        [Authorize]
        [HttpPut("complete-status/{deliveryId}")]
        public async Task<IActionResult> CompleteDeliveryStatus(Guid deliveryId)
        {
            var result = await _deliveryShipperService.CompleteDeliveryStatusAsync(deliveryId);

            if (!result)
                return BadRequest("Cập nhật trạng thái giao hàng thành công thất bại.");

            return Ok(new { message = "Trạng thái giao hàng đã được cập nhật thành 'COMPLETED'." });
        }

        [Authorize]
        [HttpPut("fail-status/{deliveryId}")]
        public async Task<IActionResult> FailDeliveryStatus(Guid deliveryId)
        {
            var result = await _deliveryShipperService.FailDeliveryStatusAsync(deliveryId);

            if (!result)
                return BadRequest("Cập nhật trạng thái giao hàng thất bại.");

            return Ok(new { message = "Trạng thái giao hàng đã được cập nhật thành 'FAILED'." });
        }
    }
}
