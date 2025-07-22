using FoodioAPI.DTOs.DeliveryShipper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace FoodieAPII.Pages.Shipper
{
    public class ShipperManagementModel : PageModel
    {
        public List<DeliveryShipperDTO> Deliveries { get; set; } = new();

        public async Task OnGetAsync([FromServices] IHttpClientFactory httpClientFactory)
        {
            var client = httpClientFactory.CreateClient();

            // Gọi API đã sửa đúng route để lấy danh sách các đơn hàng đã giao
            var resp = await client.GetAsync("https://localhost:5001/api/shipper/deliveries/delivered");
            if (resp.IsSuccessStatusCode)
            {
                // Đọc dữ liệu từ API
                Deliveries = await resp.Content.ReadFromJsonAsync<List<DeliveryShipperDTO>>() ?? new List<DeliveryShipperDTO>();
            }
            else
            {
                // Xử lý lỗi nếu API không trả về thành công
                Deliveries = new List<DeliveryShipperDTO>(); // Trả về danh sách rỗng nếu có lỗi
            }
        }
    }
}
