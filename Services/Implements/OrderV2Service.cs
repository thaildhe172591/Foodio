using FoodioAPI.Constants;
using FoodioAPI.Database;
using FoodioAPI.DTOs;
using FoodioAPI.DTOs.Order;
using FoodioAPI.Entities;
using FoodioAPI.Services;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;

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

        public async Task<List<OrderSummaryDTO>> GetOrderSummariesCashAsync(string userName)
        {
            User user = _context.User.FirstOrDefault(x => x.UserName == userName);
            if (user == null)
            {
                return null;
            }
            var orders = await _context.Orders
                .Include(o => o.DeliveryInfo)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.OrderShippers)
                .Include(o => o.Status)
                .Where(o => o.OrderTypeId == Guid.Parse("672e29b5-bd2c-4008-9117-d077cc9585d5"))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            return orders.Select(o => new OrderSummaryDTO
            {
                OrderId = o.Id,
                OrderCode = "ORD" + o.Id.ToString().Substring(0, 8).ToUpper(),
                CreatedAt = o.CreatedAt,
                Status = o.Status.Name,
                Total = o.Total,
                isAssignmentShip = o.OrderShippers.Any(),
                Adress = o.DeliveryInfo?.DeliveryAddress,
                Foods = o.OrderItems?.Select(x => x.MenuItem.Name).ToList()
            }).ToList();
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

        public async Task<Response> CreateOrderByTableAsync(CreateOrderByTableRequestDTO request, string userName)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Get DINE_IN order type
                var orderType = await _context.OrderTypes
                    .FirstOrDefaultAsync(ot => ot.Code == "DINEIN");
                
                if (orderType == null)
                {
                    return new Response
                    {
                        Status = ResponseStatus.ERROR,
                        Message = "Loại đơn hàng DINEIN không tồn tại"
                    };
                }

                DiningTable diningTable = _context.DiningTables.FirstOrDefault(x => x.TableNumber == request.TableNumber);

                if (diningTable == null)
                {
                    return new Response
                    {
                        Status = ResponseStatus.ERROR,
                        Message = "Số bàn không tồn tại"
                    };
                }
                else if (!diningTable.Status.Equals("Empty"))
                {
                    return new Response
                    {
                        Status = ResponseStatus.ERROR,
                        Message = "Bàn đang có đơn chưa xử lý xong không thể đặt thêm"
                    };
                }

                // 2. Get cashier user
                User user = _context.User.FirstOrDefault(x => x.UserName == userName);
                if (user == null)
                {
                    return new Response
                    {
                        Status = ResponseStatus.ERROR,
                        Message = "Cashier không hợp lệ"
                    };
                }

                // 3. Validate MenuItems và tính tổng tiền
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

                // 4. Get default status (PENDING)
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

                // 5. Create Order
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id, // Cashier ID
                    OrderTypeId = orderType.Id,
                    StatusId = defaultStatus.Id,
                    Total = total,
                    CreatedAt = DateTime.UtcNow,
                    TableId = diningTable.Id
                };

                _context.Orders.Add(order);

                // 6. Create OrderItems
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

                        // Add status history for order item
                        var orderItemStatusHistory = new OrderItemStatusHistory
                        {
                            Id = Guid.NewGuid(),
                            ChangedAt = DateTime.UtcNow,
                            OrderItemId = orderItem.Id,
                            StatusId = Guid.Parse("9f9d25c6-f53c-460d-99c0-ac76e015249e") // Default status
                        };
                        _context.OrderItemStatusHistories.Add(orderItemStatusHistory);
                    }
                }

                // 7. Create table info in delivery info (reusing the structure)
                var tableInfo = new OrderDeliveryInfo
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ReceiverName = request.CustomerName ?? "Khách tại bàn",
                    ReceiverPhone = request.CustomerPhone ?? "",
                    DeliveryAddress = $"Bàn số {request.TableNumber}"
                };

                _context.OrderDeliveryInfos.Add(tableInfo);

                // Cập nhật bàn
                diningTable.Status = "Book";
                _context.DiningTables.Update(diningTable);

                TableOrder tableOrder = new TableOrder
                {
                    Id = Guid.NewGuid(),
                    TableId = diningTable.Id,
                    OrderId = order.Id
                };

                _context.TableOrders.Add(tableOrder);

                // 8. Save all changes
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new Response
                {
                    Status = ResponseStatus.SUCCESS,
                    Message = "Tạo đơn hàng tại bàn thành công",
                    Data = new
                    {
                        OrderId = order.Id,
                        Total = total,
                        OrderCode = $"ORD{order.Id.ToString().Substring(0, 8).ToUpper()}",
                        TableNumber = request.TableNumber
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

        public async Task<Response?> AddShipOrder(AddShipOrderRequestDTO addShipOrderRequest)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Validate OrderType
                var user = await _context.User
                    .FirstOrDefaultAsync(ot => ot.Id == addShipOrderRequest.ShipId);

                if (user == null)
                {
                    return new Response
                    {
                        Status = ResponseStatus.ERROR,
                        Message = "user không hợp lệ"
                    };
                }
                Order order = _context.Orders.FirstOrDefault(x => x.Id == addShipOrderRequest.OrderId);
                if (order == null)
                {
                    return new Response
                    {
                        Status = ResponseStatus.ERROR,
                        Message = "order không hợp lệ"
                    };
                }

                // 2. Tạo OrderShipper
                var orderShipper = new OrderShipper
                {
                    Id = Guid.NewGuid(),
                    OrderId = addShipOrderRequest.OrderId,
                    ShipperId = addShipOrderRequest.ShipId,
                    AssignedAt = DateTime.UtcNow
                };

                _context.OrderShippers.Add(orderShipper);

                Delivery delivery = new Delivery
                {
                    Id = Guid.NewGuid(),
                    Fee = 0,
                    StatusId = Guid.Parse("c65243e7-e815-4461-bd2b-7ae39b48077d"),
                    OrderId = addShipOrderRequest.OrderId,
                    ShipperId = addShipOrderRequest.ShipId
                };

                _context.Deliveries.Add(delivery);

                // 3. Lưu tất cả vào database
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new Response
                {
                    Status = ResponseStatus.SUCCESS,
                    Message = "Update đơn hàng thành công",
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

        public async Task<List<DiningTableDTO>> GetDiningTablesAsync()
        {
            var tables = await _context.DiningTables
                .Include(dt => dt.Orders.Where(o => o.Status.Name != "Completed" && o.Status.Name != "Cancelled"))
                .OrderBy(dt => dt.TableNumber)
                .Select(dt => new DiningTableDTO
                {
                    Id = dt.Id,
                    TableNumber = dt.TableNumber,
                    Status = dt.Status,
                    IsOccupied = dt.Orders.Any(o => o.Status.Name != "Completed" && o.Status.Name != "Cancelled"),
                    HasActiveOrder = dt.Orders.Any(o => o.Status.Name != "Completed" && o.Status.Name != "Cancelled"),
                    LastOrderTime = dt.Orders
                        .Where(o => o.Status.Name != "Completed" && o.Status.Name != "Cancelled")
                        .OrderByDescending(o => o.CreatedAt)
                        .Select(o => o.CreatedAt)
                        .FirstOrDefault(),
                    ActiveOrderCount = dt.Orders.Count(o => o.Status.Name != "Completed" && o.Status.Name != "Cancelled")
                })
                .ToListAsync();

            return tables;
        }

        public async Task<TableOrderDetailDTO?> GetOrderByTableAsync(int tableNumber)
        {
            try
            {
                // Find the table
                var diningTable = await _context.DiningTables
                    .FirstOrDefaultAsync(dt => dt.TableNumber == tableNumber);

                if (diningTable == null)
                {
                    return null;
                }

                // Find active order for this table (not DELIVERED, COMPLETED, or CANCELLED)
                var order = await _context.Orders
                    .Include(o => o.Status)
                    .Include(o => o.DeliveryInfo)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                    .Include(o => o.OrderItems)
                    .Where(o => o.TableId == diningTable.Id && 
                               o.Status.Code != "DELIVERED"
                               )
                    .OrderByDescending(o => o.CreatedAt)
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    return null;
                }

                foreach (var item in order.OrderItems)
                {
                    item.StatusHistories = _context.OrderItemStatusHistories
                        .Include(x=> x.OrderItemStatus)
                        .Where(x => x.OrderItemId == item.Id).ToList();
                }

                // Map to DTO
                var orderDetail = new TableOrderDetailDTO
                {
                    OrderId = order.Id,
                    OrderCode = "ORD" + order.Id.ToString().Substring(0, 8).ToUpper(),
                    TableNumber = tableNumber,
                    CreatedAt = order.CreatedAt,
                    Total = order.Total,
                    Status = order.Status?.Name ?? "Unknown",
                    CustomerName = order.DeliveryInfo?.ReceiverName,
                    CustomerPhone = order.DeliveryInfo?.ReceiverPhone,
                    Items = order.OrderItems.Select(oi => new TableOrderItemDTO
                    {
                        MenuItemId = oi.MenuItemId,
                        MenuItemName = oi.MenuItem?.Name ?? "Unknown",
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        TotalPrice = oi.UnitPrice * oi.Quantity,
                        Note = oi.Note,
                        ItemStatus = oi.StatusHistories.Any() ?  oi.StatusHistories.FirstOrDefault().OrderItemStatus.Name : "",
                    }).ToList()
                };

                return orderDetail;
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return null;
            }
        }
    }
} 