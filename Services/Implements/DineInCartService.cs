using FoodioAPI.Constants;
using FoodioAPI.Database.Repositories;
using FoodioAPI.DTOs.DinningMenu;
using FoodioAPI.Entities;

namespace FoodioAPI.Services.Implements
{
    public class DineInCartService : IDineInCartService
    {
        private readonly IUnitOfWork _unit;

        public DineInCartService(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task AddOrUpdateCartItemAsync(Guid tableId, DineInCartItemCreateDTO dto)
        {
            var cart = await _unit.Repository<Cart>()
                .FirstOrDefaultAsync(x => x.TableId == tableId && !x.IsOrdered);

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    TableId = tableId,
                    Type = "DineIn",
                    CreatedAt = DateTime.UtcNow,
                    IsOrdered = false
                };
                await _unit.Repository<Cart>().AddAsync(cart);
            }

            var existingItem = await _unit.Repository<CartItem>()
                .FirstOrDefaultAsync(x => x.CartId == cart.Id && x.MenuItemId == dto.MenuItemId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                existingItem.Note = dto.Note ?? existingItem.Note;
            }
            else
            {
                var newItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    MenuItemId = dto.MenuItemId,
                    Quantity = dto.Quantity,
                    Note = dto.Note
                };
                await _unit.Repository<CartItem>().AddAsync(newItem);
            }

            await _unit.Save(CancellationToken.None);
        }

        public async Task<List<DineInCartItemResponseDTO>> GetCartAsync(Guid tableId)
        {
            var cart = await _unit.Repository<Cart>()
                .FirstOrDefaultAsync(x => x.TableId == tableId && !x.IsOrdered, "CartItems.MenuItem");

            if (cart == null) return new List<DineInCartItemResponseDTO>();

            return cart.CartItems.Select(item => new DineInCartItemResponseDTO
            {
                MenuItemId = item.MenuItemId,
                Name = item.MenuItem.Name,
                UnitPrice = item.MenuItem.Price,
                Quantity = item.Quantity,
                Note = item.Note
            }).ToList();
        }

        public async Task<decimal> GetTotalAsync(Guid tableId)
        {
            var cart = await _unit.Repository<Cart>()
                .FirstOrDefaultAsync(x => x.TableId == tableId && !x.IsOrdered, "CartItems.MenuItem");

            if (cart == null) return 0;

            return cart.CartItems.Sum(x => x.Quantity * x.MenuItem.Price);
        }

        public async Task<Guid> PlaceOrderAsync(Guid tableId)
        {
            var cart = await _unit.Repository<Cart>()
                .FirstOrDefaultAsync(x => x.TableId == tableId && !x.IsOrdered, "CartItems.MenuItem");

            if (cart == null || !cart.CartItems.Any())
                throw new Exception("Cart is empty or does not exist.");

            var order = new Order
            {
                Id = Guid.NewGuid(),
                TableId = tableId,
                UserId = cart.UserId,
                OrderTypeId = OrderTypeConstants.DINEIN,
                StatusId = OrderStatusConstants.PENDING,
                Total = cart.CartItems.Sum(item => item.Quantity * item.MenuItem.Price),
                CreatedAt = DateTime.UtcNow
            };

            foreach (var item in cart.CartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    MenuItemId = item.MenuItemId,
                    Quantity = item.Quantity,
                    UnitPrice = item.MenuItem.Price,
                    Note = item.Note
                });
            }

            await _unit.Repository<Order>().AddAsync(order);

            foreach (var orderItem in order.OrderItems)
            {
                await _unit.Repository<OrderItemStatusHistory>().AddAsync(new OrderItemStatusHistory
                {
                    Id = Guid.NewGuid(),
                    OrderItemId = orderItem.Id,
                    StatusId = OrderItemStatusConstants.PENDING,
                    ChangedAt = DateTime.UtcNow
                });
            }

            cart.IsOrdered = true;

            await _unit.Save(CancellationToken.None);

            return order.Id;
        }


    }

}
