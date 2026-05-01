using InvokerTraining.Application.DataTransfers;
using InvokerTraining.Models;
using InvokerTraining.Models.Entities;

namespace InvokerTraining.Application.Abstractions
{
    public interface IPlayerService
    {
        Task<Result<Player>> Register(RegisterationRequest request);
        Task<Result<TokensPair>> Login(LoginRequest request);
        Task<Result<TokensPair?>> TryRefresh(string refresh);
    }
}
