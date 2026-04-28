using InvokerTraining.Application.Abstractions;
using InvokerTraining.Application.DataTransfers;
using InvokerTraining.Infrastructure.Redis;
using InvokerTraining.Models.Entities;

namespace InvokerTraining.Application.APIServices
{
    public class PlayerService : IPlayerService
    {
        private readonly IHasher _Hasher;
        private readonly IPlayerRepository _playerRepository;
        private readonly ICacher _cacher;
        private readonly IJwtProvider _jwtProvider;

        public PlayerService(IJwtProvider jwtProvider,
            IHasher hasher,
            IPlayerRepository playerRepository,
            ICacher cacher)
        {
            _jwtProvider = jwtProvider;
            _Hasher = hasher;
            _playerRepository = playerRepository;
            _cacher = cacher;
        }

        public async Task<TokensPair> Login(LoginRequest request)
        {
            var player = await _playerRepository.GetPlayerByPhoneOrEmailAsync(request.EmailOrPhoneNumber);
            if(player == null)
            {
                throw new Exception("Fail to Login");
            }
            bool result = _Hasher.Verify(player.PasswordHash,request.password);
            if(result == false)
            {
                throw new Exception("Fail to Login");
            } 
            var acessToken = _jwtProvider.GenerateAccessToken(player.Id);
            var refreshToken = _jwtProvider.GenerateRefreshToken();
            await _cacher.Set(refreshToken,player.Id,TimeSpan.FromMinutes(2));//1 minutes Test!!!!!!!!!!!!!
            return new TokensPair(acessToken,refreshToken);
        }
        public async Task Register(RegisterationRequest request)
        {
            var player = await _playerRepository.GetPlayerByPhoneOrEmailAsync(request.EmailOrPhoneNumber);
            if(player != null)
            {
                throw new Exception("Player already registered");
            }
            string hash = _Hasher.Hash(request.password);
            var user = new Player(request.EmailOrPhoneNumber, hash);
            await _playerRepository.RegistrationAsync(user);
        }
        public async Task<TokensPair?> TryRefresh(string refresh)
        {
            var playerId = await _cacher.Get<Guid>(refresh);
            if(playerId != Guid.Empty)
            {
                var newAccessToken = _jwtProvider.GenerateAccessToken(playerId);
                await _cacher.RemoveAsync(refresh);
                var newRefreshToken = _jwtProvider.GenerateRefreshToken();
                await _cacher.Set(newRefreshToken, playerId, TimeSpan.FromMinutes(2));//1 minutes Test!!!!!!!!!!!!!
                return new TokensPair(newAccessToken,newRefreshToken);
            }
            return null;
        }
    }
}
