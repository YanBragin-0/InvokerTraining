using InvokerTraining.Models.Entities;

namespace InvokerTraining.Application.Abstractions
{
    public interface IGameSessionRepository
    {
        Task AddAsync(GameSession gameSession);
    }
}
