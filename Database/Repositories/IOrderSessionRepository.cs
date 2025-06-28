using FoodioAPI.Entities;

namespace FoodioAPI.Database.Repositories
{
    public interface IOrderSessionRepository : IBaseRepository<OrderSession>
    {
        Task<OrderSession?> GetActiveSessionByTokenAsync(string token);
        Task<OrderSession> CreateSessionAsync(Guid tableId);
    }

}
