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
    }

}
