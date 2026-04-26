using System.Diagnostics.CodeAnalysis;

namespace InvokerTraining.Models.Entities
{
    public class GameSession
    {
        public Guid Id { get; set; }
        public required Guid PlayerId { get; set; }
        public TimeSpan Time { get; set; }
        private GameSession() { }
        [SetsRequiredMembers]
        public GameSession(Guid playerId,TimeSpan sessionTime)
        {
            Id = Guid.NewGuid();
            PlayerId = playerId;
            Time = sessionTime;
        }
    }
}
