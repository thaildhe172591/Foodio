using FoodioAPI.DTOs.Menu;
using FoodioAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetMenuItems([FromBody] FilterMenuItemDto filter)
        {
            var items = await _menuService.GetMenuItemsAsync(filter);
            return Ok(items);
        }
    }
}
