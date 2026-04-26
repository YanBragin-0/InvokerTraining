using InvokerTraining.Application.DataTransfers;

namespace InvokerTraining.Application.Abstractions
{
    public interface IPlayerService
    {
        Task Register(RegisterationRequest request);
        Task<string> Login(LoginRequest request);
    }
}
