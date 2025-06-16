using FoodioAPI.Database;
using FoodioAPI.DTOs.Order;
using FoodioAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace FoodioAPI.Services.Implements;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderStatusDto>> GetOrderStatusesAsync(string userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .Select(o => new OrderStatusDto
            {
                OrderId = o.Id,
                Status = o.Status.Name,
                UpdatedAt = o.CreatedAt
            })
            .ToListAsync();
    }
}
