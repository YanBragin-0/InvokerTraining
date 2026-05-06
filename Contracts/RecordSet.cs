namespace Contracts
{
    public class RecordSet
    {
        public Guid PlayerId { get; init; }
        public TimeSpan? Time { get; init; }
        public DateTime When { get; init; }
        public string? AccountName { get; init; }
    }
 
}
