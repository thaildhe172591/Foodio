using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FoodioAPI.DTOs;


namespace FoodioAPI.Pages.Auth
{
    public class loginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public loginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [ViewData]
        public string ErrorMessage { get; set; }

        [ViewData] public string Message { get; set; } = string.Empty;
        public void OnGet(string? email = null, string? msg = null)
        {
            if (!string.IsNullOrEmpty(msg)) Message = msg;
            if (!string.IsNullOrEmpty(email)) Email = email;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both email and password.";
                return Page();
            }

            var client = _httpClientFactory.CreateClient();

            var loginPayload = new
            {
                Email,
                Password
            };

            var content = new StringContent(JsonSerializer.Serialize(loginPayload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:5001/api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Index");
            }

            //var result = await response.Content.ReadAsStringAsync();
            //ErrorMessage = $"Login failed: {result}";
            var resultJson = await response.Content.ReadAsStringAsync();

            try
            {
                var result = JsonSerializer.Deserialize<Response>(resultJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                Message = result?.Message ?? "Đăng nhập thất bại.";
            }
            catch
            {
                Message = "Đăng nhập thất bại. Lỗi không chắc chắn.";
            }
            return Page();
        }
    }
}
