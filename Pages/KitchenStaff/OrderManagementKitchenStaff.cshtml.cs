using FoodioAPI.DTOs.KitchenStaff;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodieAPII.Pages.KitchenStaff
{
    public class OrderManagementKitchenStaffModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderManagementKitchenStaffModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public List<OrderWithDetails> HotPendingOrders { get; set; } = new();
        public List<OrderWithDetails> DrinkPendingOrders { get; set; } = new();
        public List<OrderWithDetails> ColdPendingOrders { get; set; } = new();
        public List<OrderWithDetails> HotCookingOrders { get; set; } = new();
        public List<OrderWithDetails> ColdCookingOrders { get; set; } = new();
        public List<OrderWithDetails> DrinkCookingOrders { get; set; } = new();
        public List<OrderWithDetails> ReadyToServeOrders { get; set; } = new();

        public async Task OnGetAsync()
        {
            ColdPendingOrders = await GetFromApiAsync<List<OrderWithDetails>>("http://localhost:5000/api/kitchenstaff/orders/pending")
                ?? new List<OrderWithDetails> { new() { MenuItemName = "Lỗi: Không thể tải món lạnh" } };
            HotPendingOrders = await GetFromApiAsync<List<OrderWithDetails>>("http://localhost:5000/api/kitchenstaff/orders/pending/hot")
                ?? new List<OrderWithDetails> { new() { MenuItemName = "Lỗi: Không thể tải món nóng" } };
            DrinkPendingOrders = await GetFromApiAsync<List<OrderWithDetails>>("http://localhost:5000/api/kitchenstaff/orders/pending/drinks")
                ?? new List<OrderWithDetails> { new() { MenuItemName = "Lỗi: Không thể tải nước uống" } };
            HotCookingOrders = await GetFromApiAsync<List<OrderWithDetails>>("http://localhost:5000/api/kitchenstaff/orders/cooking/hot")
                ?? new();
            ColdCookingOrders = await GetFromApiAsync<List<OrderWithDetails>>("http://localhost:5000/api/kitchenstaff/orders/cooking/cold")
                ?? new();
            DrinkCookingOrders = await GetFromApiAsync<List<OrderWithDetails>>("http://localhost:5000/api/kitchenstaff/orders/cooking/drinks")
                ?? new();
            ReadyToServeOrders = await GetFromApiAsync<List<OrderWithDetails>>("http://localhost:5000/api/kitchenstaff/ready-to-serve-orders")
                ?? new List<OrderWithDetails> { new() { MenuItemName = "Lỗi: Không thể tải READY_TO_SERVE" } };



        }

        private async Task<T?> GetFromApiAsync<T>(string url)
        {
            try
            {
                var response = await _httpClientFactory.CreateClient().GetAsync(url);
                if (!response.IsSuccessStatusCode) return default;
                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[DEBUG] API content from {url}: {content}");
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch
            {
                return default;
            }
        }
    }
}
