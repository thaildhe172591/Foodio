using FoodioAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodioAPI.Controllers
{
    [ApiController]
    [Route("api/dinein/menu")]
    public class DineInMenuController : ControllerBase
    {
        private readonly IDineInMenuService _menuService;

        public DineInMenuController(IDineInMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMenu()
        {
            if (!Request.Headers.TryGetValue("access-token-key", out var token))
            {
                return BadRequest("Access token key is missing.");
            }

            var result = await _menuService.GetMenuGroupedByCategoryAsync();

            return Ok(result);
        }
    }
}
