using InvokerTraining.Application.Abstractions;
using InvokerTraining.Application.DataTransfers;
using InvokerTraining.Models.Entities;

namespace InvokerTraining.Application.APIServices
{
    public class PlayerService : IPlayerService
    {
        private readonly IHasher _Hasher;
        private readonly IPlayerRepository _playerRepository;
        private readonly IJwtProvider _jwtProvider;

        public PlayerService(IJwtProvider jwtProvider,IHasher hasher,IPlayerRepository playerRepository)
        {
            _jwtProvider = jwtProvider;
            _Hasher = hasher;
            _playerRepository = playerRepository;
        }

        public async Task<string> Login(LoginRequest request)
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
            var token = _jwtProvider.GenerateToken(player);
            return token;

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
    }
}
