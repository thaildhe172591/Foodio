using FoodioAPI.Constants;
using FoodioAPI.DTOs;
using FoodioAPI.DTOs.Order;
using FoodioAPI.Entities;
using FoodioAPI.Database;
using Microsoft.EntityFrameworkCore;
using FoodioAPI.Services;

namespace FoodioAPI.Services.Implements
{
    public class OrderV2Service : IOrderV2Service
    {
        private readonly ApplicationDbContext _context;

        public OrderV2Service(ApplicationDbContext context)
        {
            _context = context;
        }

        // Services/Implements/OrderV2Service.cs
        public async Task<OrderDetailDTO?> GetOrderDetailAsync(Guid orderId, string userName)
        {
            User user = _context.User.FirstOrDefault(x => x.UserName == userName);
            if (user == null)
            {
                return null;
            }
            var order = await _context.Orders
                .Include(o => o.Status)
                .Include(o => o.DeliveryInfo)
                .Include(o => o.OrderShippers)
                    .ThenInclude(os => os.Shipper)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == user.Id);

            if (order == null) return null;

            var shipper = order.OrderShippers.FirstOrDefault()?.Shipper;

            return new OrderDetailDTO
            {
                OrderId = order.Id,
                OrderCode = "ORD" + order.Id.ToString().Substring(0, 8).ToUpper(),
                CreatedAt = order.CreatedAt,
                Total = order.Total,
                Status = order.Status?.Name ?? "",
                DeliveryName = order.DeliveryInfo?.ReceiverName,
                DeliveryAddress = order.DeliveryInfo?.DeliveryAddress,
                DeliveryPhone = order.DeliveryInfo?.ReceiverPhone,
                DeliveryNote = order.DeliveryInfo?.DeliveryAddress, // Nếu có trường Note thì sửa lại
                ShipperName = shipper?.UserName,
                ShipperPhone = shipper?.PhoneNumber
            };
        }

        // Services/Implements/OrderV2Service.cs
        public async Task<List<OrderSummaryDTO>> GetOrderSummariesAsync(string userName)
        {
            User user = _context.User.FirstOrDefault(x => x.UserName == userName);
            if (user == null)
            {
                return null;
            }
            var orders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderSummaryDTO
                {
                    OrderId = o.Id,
                    OrderCode = "ORD" + o.Id.ToString().Substring(0, 8).ToUpper(),
                    CreatedAt = o.CreatedAt,
                    Status = o.Status.Name,
                    Total = o.Total
                })
                .ToListAsync();

            return orders;
        }

        public async Task<Response> CreateOrderAsync(CreateOrderRequestDTO request, string userName)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Validate OrderType
                var orderType = await _context.OrderTypes
                    .FirstOrDefaultAsync(ot => ot.Code == request.OrderType);
                
                if (orderType == null)
                {
                    return new Response
                    {
                        Status = ResponseStatus.ERROR,
                        Message = "Loại đơn hàng không hợp lệ"
                    };
                }
                User user = _context.User.FirstOrDefault(x=> x.UserName == userName);
                if (user == null)
                {
                    return new Response
                    {
                        Status = ResponseStatus.ERROR,
                        Message = "user không hợp lệ"
                    };
                }
                // 2. Validate MenuItems và tính tổng tiền
                var menuItemIds = request.Items.Select(i => i.MenuItemId).ToList();
                var menuItems = await _context.MenuItems
                    .Where(mi => menuItemIds.Contains(mi.Id) && mi.IsAvailable)
                    .ToDictionaryAsync(mi => mi.Id, mi => mi);

                if (menuItems.Count != menuItemIds.Count)
                {
                    return new Response
                    {
                        Status = ResponseStatus.ERROR,
                        Message = "Một số sản phẩm không tồn tại hoặc đã hết hàng"
                    };
                }

                decimal total = 0;
                foreach (var item in request.Items)
                {
                    if (menuItems.TryGetValue(item.MenuItemId, out var menuItem))
                    {
                        total += menuItem.Price * item.Quantity;
                    }
                }

                // 3. Lấy trạng thái đơn hàng mặc định (PENDING)
                var defaultStatus = await _context.OrderStatuses
                    .FirstOrDefaultAsync(os => os.Code == "PENDING");
                
                if (defaultStatus == null)
                {
                    return new Response
                    {
                        Status = ResponseStatus.ERROR,
                        Message = "Không tìm thấy trạng thái đơn hàng mặc định"
                    };
                }

                // 4. Tạo Order
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    OrderTypeId = orderType.Id,
                    StatusId = defaultStatus.Id,
                    Total = total,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);

                // 5. Tạo OrderItems
                var orderItems = new List<OrderItem>();
                foreach (var item in request.Items)
                {
                    if (menuItems.TryGetValue(item.MenuItemId, out var menuItem))
                    {
                        var orderItem = new OrderItem
                        {
                            Id = Guid.NewGuid(),
                            OrderId = order.Id,
                            MenuItemId = item.MenuItemId,
                            Quantity = item.Quantity,
                            UnitPrice = menuItem.Price,
                            Note = item.Note
                        };
                        _context.OrderItems.Add(orderItem);

                        var orderItemStatusHistory = new OrderItemStatusHistory
                        {
                            Id = Guid.NewGuid(),
                            ChangedAt = DateTime.UtcNow,
                            OrderItemId = orderItem.Id,
                            StatusId = Guid.Parse("9f9d25c6-f53c-460d-99c0-ac76e015249e")
                        };
                        _context.OrderItemStatusHistories.Add(orderItemStatusHistory);
                    }
                }


                // 6. Tạo OrderDeliveryInfo nếu là đơn giao hàng
                if (request.OrderType.ToUpper() == "DELIVERY" && request.DeliveryInfo != null)
                {
                    var deliveryInfo = new OrderDeliveryInfo
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ReceiverName = request.DeliveryInfo.ReceiverName,
                        ReceiverPhone = request.DeliveryInfo.ReceiverPhone,
                        DeliveryAddress = request.DeliveryInfo.DeliveryAddress
                    };

                    _context.OrderDeliveryInfos.Add(deliveryInfo);
                }
                else if (request.OrderType.ToUpper() == "DELIVERY" && request.DeliveryInfo == null)
                {
                    return new Response
                    {
                        Status = ResponseStatus.ERROR,
                        Message = "Thông tin giao hàng là bắt buộc cho đơn giao hàng"
                    };
                }

                // 7. Lưu tất cả vào database
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new Response
                {
                    Status = ResponseStatus.SUCCESS,
                    Message = "Tạo đơn hàng thành công",
                    Data = new
                    {
                        OrderId = order.Id,
                        Total = total,
                        OrderCode = $"ORD{order.Id.ToString().Substring(0, 8).ToUpper()}"
                    }
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new Response
                {
                    Status = ResponseStatus.ERROR,
                    Message = "Có lỗi xảy ra khi tạo đơn hàng: " + ex.Message
                };
            }
        }
    }
} 