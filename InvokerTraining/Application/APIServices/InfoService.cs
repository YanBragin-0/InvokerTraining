using InvokerTraining.Application.Abstractions;
using InvokerTraining.Application.DataTransfers;

namespace InvokerTraining.Application.APIServices
{
    public class InfoService(IPlayerRepository playerRepository) : IInfoService
    {
        private readonly IPlayerRepository _playerRepository = playerRepository;
        public async IAsyncEnumerable<PlayerResponse> GetLeaderBoardAsync()
        {
            var result = _playerRepository.GetLeadersAsync();   
            await foreach (var el in result)
            {
                yield return new PlayerResponse(el.PhoneOrEmail, el.GameCount, el.Record);
            }
        }
    }
}
