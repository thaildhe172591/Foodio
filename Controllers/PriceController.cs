using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriceController : ControllerBase
    {
        private readonly string _connectionString;

        public PriceController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllPrices()
        {
            var prices = new List<decimal>();

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = @"SELECT ""Price"" FROM dbo.""MenuItems""";

            await using var cmd = new NpgsqlCommand(query, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (reader["Price"] != DBNull.Value)
                {
                    prices.Add(reader.GetDecimal(0));
                }
            }

            return Ok(prices);
        }
    }
}
