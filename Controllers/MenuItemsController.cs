using FoodioAPI.Database;
using FoodioAPI.DTOs.UserDtos;
using FoodioAPI.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FoodioAPI.Controllers
{
    [Route("api/admin/menu-items")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class MenuItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MenuItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetMenuItems()
        {
            var menuItems = _context.MenuItems
                .Select(m => new MenuItemDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Price = m.Price,
                    ImageUrl = m.ImageUrl,
                    CategoryId = m.CategoryId,
                    IsAvailable = m.Price >= 0 // Proxy for availability
                })
                .ToList();

            return Ok(menuItems);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MenuItemDto>> GetMenuItem(Guid id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return NotFound("Menu item not found");

            var menuItemDto = new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                ImageUrl = menuItem.ImageUrl,
                CategoryId = menuItem.CategoryId,
                IsAvailable = menuItem.Price >= 0
            };

            return Ok(menuItemDto);
        }

        [HttpPost]
        public async Task<ActionResult<MenuItemDto>> CreateMenuItem([FromBody] CreateMenuItemDto dto)
        {
            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null)
                return BadRequest("Invalid category ID");

            var menuItem = new MenuItem
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.IsAvailable ? Math.Abs(dto.Price) : -Math.Abs(dto.Price),
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId
            };

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            var menuItemDto = new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                ImageUrl = menuItem.ImageUrl,
                CategoryId = menuItem.CategoryId,
                IsAvailable = menuItem.Price >= 0
            };

            return CreatedAtAction(nameof(GetMenuItem), new { id = menuItem.Id }, menuItemDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenuItem(Guid id, [FromBody] UpdateMenuItemDto dto)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return NotFound("Menu item not found");

            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null)
                return BadRequest("Invalid category ID");

            menuItem.Name = dto.Name;
            menuItem.Description = dto.Description;
            menuItem.Price = dto.IsAvailable ? Math.Abs(dto.Price) : -Math.Abs(dto.Price);
            menuItem.ImageUrl = dto.ImageUrl;
            menuItem.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}/availability")]
        public async Task<IActionResult> UpdateMenuItemAvailability(Guid id, [FromBody] bool isAvailable)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return NotFound("Menu item not found");

            menuItem.Price = isAvailable ? Math.Abs(menuItem.Price) : -Math.Abs(menuItem.Price);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(Guid id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return NotFound("Menu item not found");

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
