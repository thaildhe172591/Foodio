using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodioAPI.Pages
{
    public class WeatherModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<WeatherModel> _logger;

        public WeatherModel(IHttpClientFactory clientFactory, ILogger<WeatherModel> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public List<WeatherForecast>? Forecasts { get; set; }

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7246/WeatherForecast");

            if (response.IsSuccessStatusCode)
            {
                using var stream = await response.Content.ReadAsStreamAsync();
                Forecasts = await JsonSerializer.DeserializeAsync<List<WeatherForecast>>(stream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else
            {
                _logger.LogWarning("Weather API failed with status: {StatusCode}", response.StatusCode);
                Forecasts = new();
            }
        }
    }
}
