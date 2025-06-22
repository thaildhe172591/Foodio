using FoodioAPI.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FoodioAPI.Pages.Auth
{
    public class registerModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public registerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        [BindProperty] public string Email { get; set; } = string.Empty;
        [BindProperty] public string Password { get; set; } = string.Empty;
        [BindProperty] public string ConfirmPassword { get; set; } = string.Empty;

        [TempData] public string Message { get; set; } = string.Empty;

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || Password != ConfirmPassword)
            {
                ViewData["Message"] = "Thông tin không hợp lệ hoặc mật khẩu không khớp.";
                return Page();
            }

            var client = _httpClientFactory.CreateClient();
            var registerPayload = new RegisterDTO
            {
                Email = Email,
                Password = Password,
                ConfirmPassword = ConfirmPassword
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(registerPayload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:5001/api/auth/register", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                Message = "Đăng ký thành công. Vui lòng kiểm tra email của bạn.";
                Email = registerPayload.Email;
                ViewData["Message"] = "Đăng ký thành công. Vui lòng kiểm tra email để xác nhận.";
                return RedirectToPage("/Auth/Login", new { email = Email, msg = Message });
            }

            var responseString = await response.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(responseString);
                if (doc.RootElement.TryGetProperty("errors", out var errorsProp))
                {
                    foreach (var error in errorsProp.EnumerateObject())
                    {
                        var message = error.Value[0].GetString();
                        ViewData["Message"] = $"Đăng ký thất bại: {message}";
                        break;
                    }
                }
                else if (doc.RootElement.TryGetProperty("message", out var messageProp))
                {
                    ViewData["Message"] = $"Đăng ký thất bại: {messageProp.GetString()}";
                }
                else
                {
                    ViewData["Message"] = "Đăng ký thất bại. Vui lòng kiểm tra lại thông tin.";
                }
            }
            catch
            {
                ViewData["Message"] = "Đăng ký thất bại. Có lỗi xảy ra khi xử lý phản hồi.";
            }

            return Page();
        }
    }
}
