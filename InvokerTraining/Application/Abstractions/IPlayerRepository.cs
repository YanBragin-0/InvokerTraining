using InvokerTraining.Models.Entities;

namespace InvokerTraining.Application.Abstractions
{
    public interface IPlayerRepository
    {
        IAsyncEnumerable<Player> GetLeadersAsync();
        Task<Player?> GetPlayerByPhoneOrEmailAsync(string phoneOrEmail);
        Task<Player?> GetByIdAsync(Guid Id);
        Task RegistrationAsync(Player newPlayer);
    }
}
