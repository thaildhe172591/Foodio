using FoodioAPI.Database.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodioAPI.Controllers
{
    [ApiController]
    [Route("api/dinein/session")]
    public class QrSessionController : ControllerBase
    {
        private readonly IOrderSessionRepository _orderSessionRepo;

        public QrSessionController(IOrderSessionRepository orderSessionRepo)
        {
            _orderSessionRepo = orderSessionRepo;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSession([FromQuery] Guid tableId)
        {
            var session = await _orderSessionRepo.CreateSessionAsync(tableId);

            return Ok(new
            {
                token = session.Token,
                expiredInMinutes = 60
            });
        }
    }

}
