using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.DTOs.Order
{
    public class CreateOrderRequestDTO
    {
        [Required(ErrorMessage = "Loại đơn hàng là bắt buộc")]
        public string OrderType { get; set; } = string.Empty; // "DELIVERY", "DINE_IN", "TAKEOUT"

        [Required(ErrorMessage = "Danh sách sản phẩm không được trống")]
        public List<OrderItemRequestDTO> Items { get; set; } = new();

        public OrderDeliveryRequestDTO? DeliveryInfo { get; set; }

        public string? Note { get; set; }
    }

    public class AddShipOrderRequestDTO
    {
        [Required(ErrorMessage = "ShipId không được trống")]
        public string ShipId { get; set; }

        [Required(ErrorMessage = "OrderId không được trống")]
        public Guid OrderId { get; set; }
    }

    public class OrderItemRequestDTO
    {
        [Required(ErrorMessage = "ID sản phẩm là bắt buộc")]
        public Guid MenuItemId { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        public string? Note { get; set; }
    }

    public class OrderDeliveryRequestDTO
    {
        [Required(ErrorMessage = "Tên người nhận là bắt buộc")]
        [MaxLength(100, ErrorMessage = "Tên người nhận không được vượt quá 100 ký tự")]
        public string ReceiverName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [MaxLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string ReceiverPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        public string DeliveryAddress { get; set; } = string.Empty;
    }

    public class OrderSummaryDTO
    {
        public Guid OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public List<string> Foods { get; set; }
        public string Adress { get; set; } = string.Empty;
        public bool isAssignmentShip { get; set; }
    }

    public class OrderDetailDTO
    {
        public Guid OrderId { get; set; }
        public string OrderCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public string DeliveryName { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryPhone { get; set; }
        public string DeliveryNote { get; set; }
        public string ShipperName { get; set; }
        public string ShipperPhone { get; set; }
    }
} 