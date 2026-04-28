using StackExchange.Redis;
using System.Text.Json;
namespace InvokerTraining.Infrastructure.Redis
{
    public class CacheManager(IConnectionMultiplexer multiplexer) : ICacher
    {
        private readonly IDatabase database = multiplexer.GetDatabase();
        public async Task<T?> Get<T>(string key)
        {
            var json = await database.StringGetAsync(key);
            return json.HasValue ? JsonSerializer.Deserialize<T>((ReadOnlySpan<byte>)json) : default;
        }
        public async Task RemoveAsync(string key)
                            => await database.KeyDeleteAsync(key);

        public async Task Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            if(expiry == null)
            {
                expiry = TimeSpan.FromDays(1);
            }
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(value);
            await database.StringSetAsync(key, (RedisValue)bytes, (Expiration)expiry);
        }
    }
}
