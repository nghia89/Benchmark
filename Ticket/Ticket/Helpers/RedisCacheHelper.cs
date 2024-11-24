namespace Ticket.Helpers;

using System;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

public class RedisCacheHelper : IRedisCacheHelper
{
    private readonly IDatabase _database;

    public RedisCacheHelper(IConnectionMultiplexer redisConnection)
    {
        _database = redisConnection.GetDatabase();
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);
        if (value.IsNullOrEmpty) return default;

        return JsonSerializer.Deserialize<T>(value);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, serializedValue, expiration);
    }

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }
    
    public async Task<bool> AcquireLockAsync(string lockKey, string lockValue, TimeSpan lockDuration)
    {
        // Sử dụng SETNX để lấy khóa. Nếu khóa đã tồn tại, sẽ không thành công.
        var lockAcquired = await _database.StringSetAsync(lockKey, lockValue, lockDuration, When.NotExists);
        return lockAcquired;
    }

    public async Task ReleaseLockAsync(string lockKey)
    {
        // Kiểm tra khóa, nếu giá trị khớp, xóa khóa
        var value = await _database.StringGetAsync(lockKey);
        if (value.IsNullOrEmpty)
            return;

        await _database.KeyDeleteAsync(lockKey);
    }
}