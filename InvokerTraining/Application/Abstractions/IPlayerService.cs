using InvokerTraining.Application.DataTransfers;

namespace InvokerTraining.Application.Abstractions
{
    public interface IPlayerService
    {
        Task Register(RegisterationRequest request);
        Task<TokensPair> Login(LoginRequest request);
        Task<TokensPair?> TryRefresh(string refresh);
    }
}
