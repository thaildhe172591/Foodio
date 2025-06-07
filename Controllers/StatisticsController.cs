using FoodioAPI.Database;
using FoodioAPI.DTOs.UserDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodioAPI.Controllers
{
    [Route("api/admin/statistics")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class StatisticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("revenue/by-order-type")]
        public async Task<ActionResult<IEnumerable<RevenueByOrderTypeDto>>> GetRevenueByOrderType(
            [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var query = _context.Orders
                .Include(o => o.OrderType)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(o => o.CreatedAt >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(o => o.CreatedAt <= endDate.Value);

            var result = await query
                .GroupBy(o => o.OrderType.Name)
                .Select(g => new RevenueByOrderTypeDto
                {
                    OrderTypeName = g.Key,
                    TotalRevenue = g.Sum(o => o.Total),
                    OrderCount = g.Count()
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("revenue/by-user")]
        public async Task<ActionResult<IEnumerable<RevenueByUserDto>>> GetRevenueByUser(
            [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(o => o.CreatedAt >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(o => o.CreatedAt <= endDate.Value);

            var result = await query
                .GroupBy(o => o.User.UserName)
                .Select(g => new RevenueByUserDto
                {
                    UserName = g.Key!,
                    TotalRevenue = g.Sum(o => o.Total),
                    OrderCount = g.Count()
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("revenue/by-time")]
        public async Task<ActionResult<IEnumerable<RevenueByTimeDto>>> GetRevenueByTime(
            [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var query = _context.Orders.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(o => o.CreatedAt >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(o => o.CreatedAt <= endDate.Value);

            var result = await query
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new RevenueByTimeDto
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(o => o.Total),
                    OrderCount = g.Count()
                })
                .OrderBy(g => g.Date)
                .ToListAsync();

            return Ok(result);
        }
    }
}
