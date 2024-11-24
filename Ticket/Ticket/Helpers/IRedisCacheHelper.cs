namespace Ticket.Helpers;

public interface IRedisCacheHelper
{
    Task<T> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task<bool> AcquireLockAsync(string lockKey, string lockValue, TimeSpan lockDuration);
    Task ReleaseLockAsync(string lockKey);
}
