using FoodioAPI.Constants;
using FoodioAPI.Database.Repositories;
using FoodioAPI.Entities;
using FoodioAPI.Exceptions;

namespace FoodioAPI.Services.Implements
{
    public class CashierOrderService : ICashierOrderService
    {
        private readonly IUnitOfWork _unit;

        public CashierOrderService(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task ConfirmOrderAsync(Guid orderId)
        {
            var orderRepo = _unit.Repository<Order>();
            var orderItemRepo = _unit.Repository<OrderItemStatusHistory>();

            var order = await orderRepo.FirstOrDefaultAsync(o => o.Id == orderId, "OrderItems");

            if (order == null)
                throw new NotFoundException(nameof(Order), orderId);

            if (order.StatusId != OrderStatusConstants.PENDING)
                throw new InvalidOperationException("Only PENDING orders can be confirmed.");

            // Cập nhật trạng thái đơn
            order.StatusId = OrderStatusConstants.CONFIRMED;

            // Log trạng thái từng món
            foreach (var item in order.OrderItems)
            {
                await orderItemRepo.AddAsync(new OrderItemStatusHistory
                {
                    Id = Guid.NewGuid(),
                    OrderItemId = item.Id,
                    StatusId = OrderItemStatusConstants.CONFIRMED,
                    ChangedAt = DateTime.UtcNow
                });
            }

            await _unit.Save(CancellationToken.None);
        }
    }
}
