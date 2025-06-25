//using FoodioAPI.Database;
//using FoodioAPI.DTOs.UserDtos;
//using Microsoft.AspNetCore.Mvc;

//namespace FoodioAPI.Controllers
//{
//    [Route("api/admin/inventory")]
//    [ApiController]
//    //[Authorize(Roles = "Admin")]
//    public class InventoryController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;

//        public InventoryController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetInventoryItems()
//        {
//            var items = await _context.InventoryItems
//                .Select(i => new InventoryItemDto
//                {
//                    Id = i.Id,
//                    Name = i.Name,
//                    Quantity = i.Quantity,
//                    Unit = i.Unit,
//                    Price = i.Price,
//                    LastUpdated = i.LastUpdated
//                })
//                .ToListAsync();

//            return Ok(items);
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<InventoryItemDto>> GetInventoryItem(Guid id)
//        {
//            var item = await _context.InventoryItems.FindAsync(id);
//            if (item == null)
//                return NotFound("Inventory item not found");

//            var itemDto = new InventoryItemDto
//            {
//                Id = item.Id,
//                Name = item.Name,
//                Quantity = item.Quantity,
//                Unit = item.Unit,
//                Price = item.Price,
//                LastUpdated = item.LastUpdated
//            };

//            return Ok(itemDto);
//        }

//        [HttpPost]
//        public async Task<ActionResult<InventoryItemDto>> CreateInventoryItem([FromBody] CreateInventoryItemDto dto)
//        {
//            var item = new InventoryItem
//            {
//                Name = dto.Name,
//                Quantity = dto.Quantity,
//                Unit = dto.Unit,
//                Price = dto.Price
//            };

//            _context.InventoryItems.Add(item);
//            await _context.SaveChangesAsync();

//            var itemDto = new InventoryItemDto
//            {
//                Id = item.Id,
//                Name = item.Name,
//                Quantity = item.Quantity,
//                Unit = item.Unit,
//                Price = item.Price,
//                LastUpdated = item.LastUpdated
//            };

//            return CreatedAtAction(nameof(GetInventoryItem), new { id = item.Id }, itemDto);
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateInventoryItem(Guid id, [FromBody] UpdateInventoryItemDto dto)
//        {
//            var item = await _context.InventoryItems.FindAsync(id);
//            if (item == null)
//                return NotFound("Inventory item not found");

//            item.Name = dto.Name;
//            item.Quantity = dto.Quantity;
//            item.Unit = dto.Unit;
//            item.Price = dto.Price;
//            item.LastUpdated = DateTime.UtcNow;

//            await _context.SaveChangesAsync();
//            return NoContent();
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteInventoryItem(Guid id)
//        {
//            var item = await _context.InventoryItems.FindAsync(id);
//            if (item == null)
//                return NotFound("Inventory item not found");

//            _context.InventoryItems.Remove(item);
//            await _context.SaveChangesAsync();
//            return NoContent();
//        }
//    }
//}
