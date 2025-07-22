using FoodioAPI.DTOs;
using FoodioAPI.DTOs.Order;
using FoodioAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartV2Controller : ControllerBase
    {
        private readonly IOrderV2Service _orderV2Service;

        public CartV2Controller(IOrderV2Service orderV2Service)
        {
            _orderV2Service = orderV2Service;
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(new Response
                {
                    Status = ResponseStatus.ERROR,
                    Message = "Không thể xác thực người dùng"
                });
            }

            var orders = await _orderV2Service.GetOrderSummariesAsync(userName);
            return Ok(new Response
            {
                Status = ResponseStatus.SUCCESS,
                Message = "Lấy danh sách đơn hàng thành công",
                Data = orders
            });
        }


        [HttpGet("my-orders-cash")]
        public async Task<IActionResult> GetMyOrdersCash()
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(new Response
                {
                    Status = ResponseStatus.ERROR,
                    Message = "Không thể xác thực người dùng"
                });
            }

            var orders = await _orderV2Service.GetOrderSummariesCashAsync(userName);
            return Ok(new Response
            {
                Status = ResponseStatus.SUCCESS,
                Message = "Lấy danh sách đơn hàng thành công",
                Data = orders
            });
        }

        /// <summary>
        /// Tạo đơn hàng từ thông tin giỏ hàng
        /// </summary>
        /// <param name="request">Thông tin đơn hàng</param>
        /// <returns>Kết quả tạo đơn hàng</returns>
        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDTO request)
        {
            try
            {
                // Lấy userId từ JWT token
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                
                if (string.IsNullOrEmpty(userName))
                {
                    return Unauthorized(new Response
                    {
                        Status = ResponseStatus.ERROR,
                        Message = "Không thể xác thực người dùng"
                    });
                }

                var result = await _orderV2Service.CreateOrderAsync(request, userName);
                
                if (result.Status == ResponseStatus.SUCCESS)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response
                {
                    Status = ResponseStatus.ERROR,
                    Message = "Lỗi server: " + ex.Message
                });
            }
        }

        [HttpGet("order-detail/{orderId}")]
        public async Task<IActionResult> GetOrderDetail(Guid orderId)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(new Response
                {
                    Status = ResponseStatus.ERROR,
                    Message = "Không thể xác thực người dùng"
                });
            }

            var detail = await _orderV2Service.GetOrderDetailAsync(orderId, userName);
            if (detail == null)
            {
                return NotFound(new Response
                {
                    Status = ResponseStatus.ERROR,
                    Message = "Không tìm thấy đơn hàng"
                });
            }

            return Ok(new Response
            {
                Status = ResponseStatus.SUCCESS,
                Message = "Lấy chi tiết đơn hàng thành công",
                Data = detail
            });
        }
    }
}
