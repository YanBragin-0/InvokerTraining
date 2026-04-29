using InvokerTraining.Application.Abstractions;
using InvokerTraining.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvokerTraining.Infrastructure.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly AppDbContext _context;

        public PlayerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Player?> GetByIdAsync(Guid Id)
        {
            var user = await _context.Players.FindAsync(Id);
            return user;
        }

        public IAsyncEnumerable<Player> GetLeadersAsync() 
                => _context.Players.OrderBy(p => p.Record).AsAsyncEnumerable().Take(20);

        public async Task<Player?> GetPlayerByPhoneOrEmailAsync(string phoneOrEmail)
        {
            var player = await _context.Players.AsNoTracking().FirstOrDefaultAsync(p => p.PhoneOrEmail == phoneOrEmail);
            return player;
        }


        public async Task RegistrationAsync(Player newPlayer)
        {
            await _context.Players.AddAsync(newPlayer);
            await _context.SaveChangesAsync();
        }

        public async Task SavechangesAsync() => await _context.SaveChangesAsync();

    }
}
