using InvokerTraining.Application.Abstractions;
using InvokerTraining.Models.Entities;

namespace InvokerTraining.Infrastructure.Repositories
{
    public class GameSessionRepository(AppDbContext context) : IGameSessionRepository
    {
        private readonly AppDbContext _context = context;
        public async Task AddAsync(GameSession gameSession)
        {
            await _context.GameSessions.AddAsync(gameSession);
            await _context.SaveChangesAsync();
        }
    }
}
