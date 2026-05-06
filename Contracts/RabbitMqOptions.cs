namespace Contracts
{
    public class RabbitMqOptions
    {
        public string AmpqChannelPort { get; init; } = null!;
        public string AdminPanelPort { get; init; } = null!;
        public string Username { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string? Host { get; set; }
    }
 
}
