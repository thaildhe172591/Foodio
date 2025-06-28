using FoodioAPI.Entities;
using Microsoft.EntityFrameworkCore;


using System;

namespace FoodioAPI.Database.Repositories.Implements
{
    public class OrderSessionRepository : BaseRepository<OrderSession>, IOrderSessionRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderSessionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<OrderSession?> GetActiveSessionByTokenAsync(string token)
        {
            return await _context.OrderSessions
                .FirstOrDefaultAsync(x => x.Token == token && x.IsActive);
        }

        public async Task<OrderSession> CreateSessionAsync(Guid tableId)
        {
            var token = Guid.NewGuid().ToString("N");
            var session = new OrderSession
            {
                Id = Guid.NewGuid(),
                Token = token,
                TableId = tableId,
                IsActive = true,
                ExpireAt = DateTime.UtcNow
            };

            await _context.OrderSessions.AddAsync(session);
            await _context.SaveChangesAsync();

            return session;
        }
    }

}
