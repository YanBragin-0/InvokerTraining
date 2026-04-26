using InvokerTraining.Application.Abstractions;

namespace InvokerTraining.Infrastructure
{
    public class PasswordHasher : IHasher
    {
        public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
        public bool Verify(string hash, string password) => BCrypt.Net.BCrypt.Verify(password,hash);
    }
}
