using InvokerTraining.Application.DataTransfers;

namespace InvokerTraining.Application.Abstractions
{
    public interface IInfoService
    {
        IAsyncEnumerable<PlayerResponse> GetLeaderBoardAsync();
    }
}
