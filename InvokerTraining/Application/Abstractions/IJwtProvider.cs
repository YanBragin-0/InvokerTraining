using InvokerTraining.Models.Entities;

namespace InvokerTraining.Application.Abstractions
{
    public interface IJwtProvider
    {
        string GenerateToken(Player player);
    }
}