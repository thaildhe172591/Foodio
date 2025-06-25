using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FoodioAPI.Configs;
using FoodioAPI.DTOs.Auth;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FoodioAPI.Pages.Auth
{
    public class GoogleCallbackModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GoogleConfig _google;
        private readonly ILogger<GoogleCallbackModel> _logger;


        public GoogleCallbackModel(IHttpClientFactory httpClientFactory, IOptions<GoogleConfig> googleConfig, ILogger<GoogleCallbackModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _google = googleConfig.Value;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string code, string? error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                TempData["Message"] = "Bạn đã hủy đăng nhập với Google.";
                return RedirectToPage("/Auth/Login");
            }

            if (string.IsNullOrEmpty(code))
            {
                TempData["Message"] = "Không nhận được mã xác thực từ Google.";
                return RedirectToPage("/Auth/Register");
            }

            var tokenReq = new Dictionary<string, string?>
            {
                ["code"] = code,
                ["client_id"] = _google.ClientId,
                ["client_secret"] = _google.ClientSecret,
                ["redirect_uri"] = _google.RedirectUri,
                ["grant_type"] = "authorization_code"
            };

            var http = _httpClientFactory.CreateClient();
            var tokenRes = await http.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(tokenReq));

            if (!tokenRes.IsSuccessStatusCode)
            {
                TempData["Message"] = "Không thể lấy token từ Google.";
                return RedirectToPage("/Auth/Register");
            }

            var tokenJson = await tokenRes.Content.ReadAsStringAsync();
            _logger.LogInformation("Google Token JSON: {json}", tokenJson);

            using var doc = JsonDocument.Parse(tokenJson);
            var idToken = doc.RootElement.GetProperty("id_token").GetString();
            _logger.LogInformation("Received id_token: {token}", idToken);

            var api = _httpClientFactory.CreateClient("FoodioAPI");
            api.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var payload = new ExternalAuthDTO
            {
                Provider = "GOOGLE",
                IdToken = idToken!
            };

            var apiRes = await api.PostAsJsonAsync("/api/auth/login-google", payload);

            if (apiRes.IsSuccessStatusCode)
                return RedirectToPage("/Index");

            TempData["Message"] = "Đăng nhập Google thất bại.";
            return RedirectToPage("/Auth/Register");
        }
    }
}
