using FoodioAPI.Database;
using FoodioAPI.DTOs.Cart;
using FoodioAPI.DTOs.Menu;
using FoodioAPI.DTOs.Order;
using FoodioAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodioAPI.Services.Implements;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _context;

    public CartService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddToCartAsync(string userId, AddToCartDto dto)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsOrdered);

        if (cart == null)
        {
            cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsOrdered = false,
                Type = "delivery", // default
                CartItems = new List<CartItem>()
            };

            _context.Carts.Add(cart);
        }

        var existingItem = cart.CartItems.FirstOrDefault(i => i.MenuItemId == dto.MenuItemId);
        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
            existingItem.Note = dto.Note ?? existingItem.Note;
        }
        else
        {
            cart.CartItems.Add(new CartItem
            {
                Id = Guid.NewGuid(),
                MenuItemId = dto.MenuItemId,
                Quantity = dto.Quantity,
                Note = dto.Note,
                CartId = cart.Id
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> AddMenuItemToCartAsync(string userId, AddMenuItemToCartDto dto)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsOrdered);

        if (cart == null)
        {
            cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsOrdered = false,
                Type = "delivery",
                CartItems = new List<CartItem>()
            };

            _context.Carts.Add(cart);
        }

        var existingItem = cart.CartItems.FirstOrDefault(i => i.MenuItemId == dto.MenuItemId);
        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
        }
        else
        {
            cart.CartItems.Add(new CartItem
            {
                Id = Guid.NewGuid(),
                MenuItemId = dto.MenuItemId,
                Quantity = dto.Quantity,
                CartId = cart.Id
            });
        }

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<List<CartItemDto>> GetCartItemsAsync(string userId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(i => i.MenuItem)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsOrdered);

        if (cart == null || !cart.CartItems.Any())
            return new List<CartItemDto>();

        return cart.CartItems.Select(i => new CartItemDto
        {
            Id = i.Id,
            MenuItemId = i.MenuItemId,
            Name = i.MenuItem.Name,
            Description = i.MenuItem.Description,
            Price = i.MenuItem.Price,
            ImageUrl = i.MenuItem.ImageUrl,
            Quantity = i.Quantity
        }).ToList();
    }

    public async Task<IEnumerable<CartItemDto>> GetCartAsync(string userId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(i => i.MenuItem)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsOrdered);

        if (cart == null) return Enumerable.Empty<CartItemDto>();

        return cart.CartItems.Select(i => new CartItemDto
        {
            Id = i.Id,
            MenuItemId = i.MenuItemId,
            Name = i.MenuItem.Name,
            ImageUrl = i.MenuItem.ImageUrl,
            Quantity = i.Quantity,
            Price = i.MenuItem.Price
        });
    }

    public async Task<bool> SubmitDeliveryInfoAsync(string userId, DeliveryInfoDto dto)
    {
        var cart = await _context.Carts
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsOrdered);

        if (cart == null)
            return false;

        cart.Type = dto.Type.ToLower();

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ConfirmOrderAsync(string userId, ConfirmOrderDto dto)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(i => i.MenuItem)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsOrdered);

        if (cart == null || !cart.CartItems.Any())
            return false;

        var orderTypeId = await _context.OrderTypes
            .Where(t => t.Name.ToLower() == dto.Type.ToLower())
            .Select(t => t.Id)
            .FirstOrDefaultAsync();

        if (orderTypeId == Guid.Empty)
            return false;

        var statusId = await _context.OrderStatuses
            .Where(s => s.Name.ToLower() == "processing")
            .Select(s => s.Id)
            .FirstOrDefaultAsync();

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OrderTypeId = orderTypeId,
            StatusId = statusId,
            CreatedAt = DateTime.UtcNow,
            Total = cart.CartItems.Sum(i => i.MenuItem.Price * i.Quantity),
            OrderItems = cart.CartItems.Select(i => new OrderItem
            {
                MenuItemId = i.MenuItemId,
                Quantity = i.Quantity
            }).ToList()
        };

        if (dto.Type.ToLower() == "delivery")
        {
            order.DeliveryInfo = new OrderDeliveryInfo
            {
                OrderId = order.Id,
                ReceiverName = dto.ReceiverName ?? "",
                ReceiverPhone = dto.ReceiverPhone ?? "",
                DeliveryAddress = dto.DeliveryAddress ?? ""
            };
        }

        _context.Orders.Add(order);
        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<bool> RemoveCartItemAsync(string userId, Guid cartItemId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsOrdered);

        if (cart == null)
            return false;

        var itemToRemove = cart.CartItems.FirstOrDefault(i => i.Id == cartItemId);
        if (itemToRemove == null)
            return false;

        cart.CartItems.Remove(itemToRemove);
        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<bool> UpdateCartItemQuantityAsync(string userId, UpdateCartItemDto dto)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsOrdered);

        if (cart == null)
            return false;

        var itemToUpdate = cart.CartItems.FirstOrDefault(i => i.Id == dto.CartItemId);
        if (itemToUpdate == null)
            return false;

        if (dto.Quantity <= 0)
        {
            cart.CartItems.Remove(itemToUpdate); // xoá nếu số lượng = 0
        }
        else
        {
            itemToUpdate.Quantity = dto.Quantity;
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
