using InvokerTraining.Models.Entities;

namespace InvokerTraining.Application.Abstractions
{
    public interface IJwtProvider
    {
        string GenerateAccessToken(Guid playerId);
        string GenerateRefreshToken();
    }
}