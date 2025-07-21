using FoodioAPI.Constants;
using FoodioAPI.Database.Repositories;
using FoodioAPI.DTOs.DinningMenu;
using FoodioAPI.Entities;
using Microsoft.EntityFrameworkCore;
using FoodioAPI.Exceptions;

namespace FoodioAPI.Services.Implements
{
    public class DineInOrderService : IDineInOrderService
    {
        private readonly IUnitOfWork _unit;

        public DineInOrderService(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<List<DineInOrderHistoryDTO>> GetOrderHistoryByTableAsync(Guid tableId)
        {
            var orders = await _unit.Repository<Order>()
                .Entities
                .Where(o => o.TableId == tableId)
                .Include(o => o.Status)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.StatusHistories.OrderByDescending(h => h.ChangedAt))
                        .ThenInclude(sh => sh.OrderItemStatus)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(o => new DineInOrderHistoryDTO
            {
                OrderId = o.Id,
                Total = o.Total,
                OrderStatus = o.Status.Name,
                CreatedAt = o.CreatedAt,
                Items = o.OrderItems.Select(oi => new DineInOrderItemStatusDTO
                {
                    MenuItemId = oi.MenuItemId,
                    Name = oi.MenuItem.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Note = oi.Note,
                    LatestStatus = oi.StatusHistories.FirstOrDefault()?.OrderItemStatus.Name ?? "Unknown",
                    StatusChangedAt = oi.StatusHistories.FirstOrDefault()?.ChangedAt ?? o.CreatedAt
                }).ToList()
            }).ToList();
        }

        public async Task RequestPaymentAsync(Guid tableId, Guid orderId)
        {
            var order = await _unit.Repository<Order>()
                .FirstOrDefaultAsync(o => o.Id == orderId && o.TableId == tableId);

            if (order == null)
                throw new NotFoundException("Order not found or does not belong to the current table.");

            if (order.StatusId != OrderStatusConstants.CONFIRMED)
                throw new InvalidOperationException("Only confirmed orders can request payment.");

            var existingRequest = await _unit.Repository<ServiceRequest>()
                .FirstOrDefaultAsync(r =>
                    r.TableId == tableId &&
                    r.Type == ServiceRequestTypeConstants.REQUEST_PAYMENT &&
                    r.Status == ServiceRequestStatusConstants.PENDING);

            if (existingRequest != null)
                throw new InvalidOperationException("You already have a pending payment request.");

            var newRequest = new ServiceRequest
            {
                Id = Guid.NewGuid(),
                TableId = tableId,
                Type = ServiceRequestTypeConstants.REQUEST_PAYMENT,
                Status = ServiceRequestStatusConstants.PENDING,
                CreatedAt = DateTime.UtcNow
            };

            await _unit.Repository<ServiceRequest>().AddAsync(newRequest);
            await _unit.Save(CancellationToken.None);
        }

        public async Task CallStaffAsync(Guid tableId, Guid orderId)
        {
            var order = await _unit.Repository<Order>()
                .FirstOrDefaultAsync(o => o.Id == orderId && o.TableId == tableId);

            if (order == null)
                throw new NotFoundException("Order not found or does not belong to the current table.");

            var existingRequest = await _unit.Repository<ServiceRequest>()
                .FirstOrDefaultAsync(r =>
                    r.TableId == tableId &&
                    r.Type == ServiceRequestTypeConstants.CALL_STAFF &&
                    r.Status == ServiceRequestStatusConstants.PENDING);

            if (existingRequest != null)
                throw new InvalidOperationException("You already have a pending staff request.");

            var newRequest = new ServiceRequest
            {
                Id = Guid.NewGuid(),
                TableId = tableId,
                Type = ServiceRequestTypeConstants.CALL_STAFF,
                Status = ServiceRequestStatusConstants.PENDING,
                CreatedAt = DateTime.UtcNow
            };

            await _unit.Repository<ServiceRequest>().AddAsync(newRequest);
            await _unit.Save(CancellationToken.None);
        }

    }
}
