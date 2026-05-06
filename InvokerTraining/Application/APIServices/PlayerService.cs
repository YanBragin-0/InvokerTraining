using InvokerTraining.Application.Abstractions;
using InvokerTraining.Application.DataTransfers;
using InvokerTraining.Infrastructure.JWT;
using InvokerTraining.Infrastructure.Redis;
using InvokerTraining.Models;
using InvokerTraining.Models.Entities;
using Microsoft.Extensions.Options;

namespace InvokerTraining.Application.APIServices
{
    public class PlayerService : IPlayerService
    {
        private readonly IHasher _Hasher;
        private readonly IPlayerRepository _playerRepository;
        private readonly ICacher _cacher;
        private readonly IOptions<JwtOptions> options;
        private readonly IJwtProvider _jwtProvider;

        public PlayerService(IJwtProvider jwtProvider,
            IHasher hasher,
            IPlayerRepository playerRepository,
            ICacher cacher,
            IOptions<JwtOptions> options)
        {
            _jwtProvider = jwtProvider;
            _Hasher = hasher;
            _playerRepository = playerRepository;
            _cacher = cacher;
            this.options = options;
        }

        public async Task<Result<TokensPair>> Login(LoginRequest request)
        {
            var player = await _playerRepository.GetPlayerByPhoneOrEmailAsync(request.EmailOrPhoneNumber);
            if(player == null)
            {
                return Result<TokensPair>.Error("Login error(wrong email or phone number). Try again");
            }
            bool result = _Hasher.Verify(player.PasswordHash,request.password);
            if(result == false)
            {
                return Result<TokensPair>.Error("Login error(wrong password). Try again");
            } 
            var acessToken = _jwtProvider.GenerateAccessToken(player.Id);
            var refreshToken = _jwtProvider.GenerateRefreshToken();
            await _cacher.Set(refreshToken,player.Id,TimeSpan.FromHours(options.Value.Refresh));
            return Result<TokensPair>.Success(new TokensPair(acessToken,refreshToken));
        }

        public async Task Logout(string token) => await _cacher.RemoveAsync(token);

        public async Task<Result<Player>> Register(RegisterationRequest request)
        {
            var player = await _playerRepository.GetPlayerByPhoneOrEmailAsync(request.EmailOrPhoneNumber);
            if(player != null)
            {
                return Result<Player>.Error("Registration error(player already registered). Try again");
            }
            string hash = _Hasher.Hash(request.password);
            var user = new Player(request.EmailOrPhoneNumber, hash);
            await _playerRepository.RegistrationAsync(user);
            return Result<Player>.Success(user);
        }
        public async Task<Result<TokensPair?>> TryRefresh(string refresh)
        {
            var playerId = await _cacher.Get<Guid>(refresh);
            if(playerId != Guid.Empty)
            {
                var newAccessToken = _jwtProvider.GenerateAccessToken(playerId);
                await _cacher.RemoveAsync(refresh);
                var newRefreshToken = _jwtProvider.GenerateRefreshToken();
                await _cacher.Set(newRefreshToken, playerId, TimeSpan.FromHours(options.Value.Refresh));
                return Result<TokensPair?>.Success(new TokensPair(newAccessToken,newRefreshToken));
            }
            return Result<TokensPair?>.Error("Error Refreshing tokens");
        }
    }
}
