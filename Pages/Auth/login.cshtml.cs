using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FoodioAPI.DTOs;
using FoodioAPI.Configs;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.WebUtilities;


namespace FoodioAPI.Pages.Auth
{
    public class loginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GoogleConfig _google;
        public loginModel(IHttpClientFactory httpClientFactory, IOptions<GoogleConfig> googleConfig)
        {
            _httpClientFactory = httpClientFactory;
            _google            = googleConfig.Value;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        [TempData] 
        public string Message { get; set; } = string.Empty;

        public bool RegisterSuccess { get; private set; } = false;
        public void OnGet(string? email = null)
        {
            if (TempData["RegisterSuccess"] != null)
            {
                RegisterSuccess = true;
                Message = TempData["Message"]?.ToString() ?? "";
            }

            if (!string.IsNullOrEmpty(email))
                Email = email;
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

        public IActionResult OnGetGoogleLogin()
        {
            var query = new Dictionary<string, string?>
            {
                ["client_id"] = _google.ClientId,
                ["redirect_uri"] = _google.RedirectUri,
                ["response_type"] = "code",
                ["scope"] = "openid email profile",
                ["access_type"] = "offline",
                ["prompt"] = "select_account"
            };

            var googleUrl = QueryHelpers.AddQueryString(
                                "https://accounts.google.com/o/oauth2/v2/auth",
                                query);

            return Redirect(googleUrl);
        }
    }
}
