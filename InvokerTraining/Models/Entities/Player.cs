using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace InvokerTraining.Models.Entities
{
    public class Player
    {
        public Guid Id { get; set; }
        public required string PhoneOrEmail { get; set; } 
        public required string PasswordHash { get; set; }
        public int GameCount { get; set; }
        public TimeSpan? Record {  get; set; }
        private Player() { }

        [SetsRequiredMembers]
        public Player(string PhoneOrEmail,string passwordHash)
        {
            Id = Guid.NewGuid();
            this.PhoneOrEmail = PhoneOrEmail;
            PasswordHash = passwordHash;
        }
    }
}
