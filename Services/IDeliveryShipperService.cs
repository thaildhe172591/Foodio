using FoodioAPI.DTOs.DeliveryShipper;
using System;
using System.Threading.Tasks;

namespace FoodioAPI.Services
{
    public interface IDeliveryShipperService
    {
        Task<List<DeliveryShipperDTO>> GetDeliveriesByShipperAsync(string username);
        Task<List<DeliveryShipperDTO>> GetAllHaveDeliveriedByShipperAsync(string username);

        // Cập nhật trạng thái đơn hàng
        Task<bool> UpdateDeliveryStatusAsync(Guid deliveryId, string status);

        // Phương thức để cập nhật trạng thái giao hàng thành công (COMPLETED)
        Task<bool> CompleteDeliveryStatusAsync(Guid deliveryId);

        // Phương thức để cập nhật trạng thái giao hàng thất bại (FAILED)
        Task<bool> FailDeliveryStatusAsync(Guid deliveryId);
    }
}
