using FoodioAPI.Database;
using FoodioAPI.DTOs.DeliveryShipper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FoodioAPI.Services.Implements
{
    public class DeliveryShipperService : IDeliveryShipperService
    {
        private readonly ApplicationDbContext _context;

        public DeliveryShipperService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<DeliveryShipperDTO>> GetDeliveriesByShipperAsync(string username)
        {
            var result = await (
                from d in _context.Deliveries
                join ds in _context.DeliveryStatuses on d.StatusId equals ds.Id
                join o in _context.Orders on d.OrderId equals o.Id
                join oi in _context.OrderItems on o.Id equals oi.OrderId
                join mi in _context.MenuItems on oi.MenuItemId equals mi.Id
                join odi in _context.OrderDeliveryInfos on o.Id equals odi.OrderId
                join ot in _context.OrderTypes on o.OrderTypeId equals ot.Id
                join u in _context.Users on o.UserId equals u.Id
                join ur in _context.UserRoles on u.Id equals ur.UserId into urJoin
                from ur in urJoin.DefaultIfEmpty()
                join r in _context.Roles on ur.RoleId equals r.Id into rJoin
                from r in rJoin.DefaultIfEmpty()
                where ot.Code == "DELIVERY"
                      && ds.Code == "PENDING"  
                      && r.Name == "Shipper"
                      && u.UserName == username
                select new DeliveryShipperDTO
                {
                    DeliveryId = d.Id,
                    ReceiverName = odi.ReceiverName,
                    MenuItemName = mi.Name,
                    ReceiverPhone = odi.ReceiverPhone,
                    DeliveryAddress = odi.DeliveryAddress,
                    Quantity = oi.Quantity,
                    Note = oi.Note,
                    Total = o.Total,
                    Fee = d.Fee,
                    CreatedAt = o.CreatedAt,
                    Code = ds.Code,
                    DeliveryStatusName = ds.Name,
                    OrderUserName = u.UserName,
                    RoleName = r.Name
                }
            ).ToListAsync();

            return result;
        }


        public async Task<List<DeliveryShipperDTO>> GetAllHaveDeliveriedByShipperAsync(string username)
        {
            var result = await (
                from d in _context.Deliveries
                join ds in _context.DeliveryStatuses on d.StatusId equals ds.Id
                join o in _context.Orders on d.OrderId equals o.Id
                join oi in _context.OrderItems on o.Id equals oi.OrderId
                join mi in _context.MenuItems on oi.MenuItemId equals mi.Id
                join odi in _context.OrderDeliveryInfos on o.Id equals odi.OrderId
                join ot in _context.OrderTypes on o.OrderTypeId equals ot.Id
                join u in _context.Users on o.UserId equals u.Id
                join ur in _context.UserRoles on u.Id equals ur.UserId into urJoin
                from ur in urJoin.DefaultIfEmpty()
                join r in _context.Roles on ur.RoleId equals r.Id into rJoin
                from r in rJoin.DefaultIfEmpty()
                where ot.Code == "DELIVERY"
                      && ds.Code == "DELIVERED"  
                      && r.Name == "Shipper"
                      && u.UserName == username
                select new DeliveryShipperDTO
                {
                    DeliveryId = d.Id,
                    ReceiverName = odi.ReceiverName,
                    MenuItemName = mi.Name,
                    ReceiverPhone = odi.ReceiverPhone,
                    DeliveryAddress = odi.DeliveryAddress,
                    Quantity = oi.Quantity,
                    Note = oi.Note,
                    Total = o.Total,
                    Fee = d.Fee,
                    CreatedAt = o.CreatedAt,
                    Code = ds.Code,
                    DeliveryStatusName = ds.Name,
                    OrderUserName = u.UserName,
                    RoleName = r.Name
                }
            ).ToListAsync();

            return result;
        }



        public async Task<bool> UpdateDeliveryStatusAsync(Guid deliveryId, string status)
        {
            // Tìm kiếm đơn giao hàng
            var delivery = await _context.Deliveries
                .FirstOrDefaultAsync(d => d.Id == deliveryId);

            if (delivery == null)
                return false;

            // Lấy trạng thái tương ứng từ cơ sở dữ liệu
            var statusEntity = await _context.DeliveryStatuses
                .FirstOrDefaultAsync(ds => ds.Code == status);

            if (statusEntity == null)
                return false;

            // Cập nhật trạng thái cho đơn giao hàng
            delivery.StatusId = statusEntity.Id;
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> CompleteDeliveryStatusAsync(Guid deliveryId)
        {
            var delivery = await _context.Deliveries
                .FirstOrDefaultAsync(d => d.Id == deliveryId);

            if (delivery == null)
                return false;

            var completedStatus = await _context.DeliveryStatuses
                .FirstOrDefaultAsync(ds => ds.Code == "DELIVERED");

            if (completedStatus == null)
                return false;

            delivery.StatusId = completedStatus.Id;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> FailDeliveryStatusAsync(Guid deliveryId)
        {
            var delivery = await _context.Deliveries
                .FirstOrDefaultAsync(d => d.Id == deliveryId);

            if (delivery == null)
                return false;

            var failedStatus = await _context.DeliveryStatuses
                .FirstOrDefaultAsync(ds => ds.Code == "FAILED");

            if (failedStatus == null)
                return false;

            delivery.StatusId = failedStatus.Id;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
