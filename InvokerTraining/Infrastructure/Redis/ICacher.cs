namespace InvokerTraining.Infrastructure.Redis
{
    public interface ICacher
    {
        Task Set<T>(string key, T value, TimeSpan? expiry = null);
        Task<T?> Get<T>(string key);
        Task RemoveAsync(string key);

    }
}
