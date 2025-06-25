using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FoodioAPI.Pages.Auth
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ForgotPasswordModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public string Email { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Email không hợp lệ.";
                return Page();
            }

            var client = _httpClientFactory.CreateClient();

            var forgotPasswordData = new { Email };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(forgotPasswordData),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("https://localhost:5001/api/auth/forgot-password", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                ViewData["Message"] = "Yêu cầu đặt lại mật khẩu đã được gửi. Vui lòng kiểm tra email.";
                return Page();
            }
            else
            {
                ViewData["Message"] = "Không thể tìm thấy email hoặc đã xảy ra lỗi.";
            }

            return Page();
        }
    }
}
