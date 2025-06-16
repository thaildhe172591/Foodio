using FoodioAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodioAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("status")]
    [Authorize]
    public async Task<IActionResult> GetStatus()
    {
        var userId = User.Identity.Name;
        var statuses = await _orderService.GetOrderStatusesAsync(userId!);
        return Ok(statuses);
    }
}
