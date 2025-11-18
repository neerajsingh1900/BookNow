using BookNow.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace BookNow.Application.Services
{
    public class RedisLockService : IRedisLockService
    {
        private readonly ConnectionMultiplexer _redis;

        public RedisLockService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("RedisConnection");
            _redis = ConnectionMultiplexer.Connect(connectionString);
        }

        public IDatabase GetDatabase() => _redis.GetDatabase();
        private const string HOLD_CLEANUP_ZSET = "seat_holds_expiry";

        public Task<bool> AcquireLockAsync(string key, string value, TimeSpan expiry)
        {
          
            return GetDatabase().StringSetAsync(key, value, expiry, When.NotExists);
        }

      
        public async Task<bool> ReleaseLockAsync(string key, string value)
        {
             var luaScript = @"
                if redis.call('get', KEYS[1]) == ARGV[1] then
                    return redis.call('del', KEYS[1])
                else
                    return 0
                end";

            RedisResult result = await GetDatabase().ScriptEvaluateAsync(
                 luaScript,
                 new RedisKey[] { key },
                 new RedisValue[] { value });

            return (long)result == 1;
        }

        public Task AddHoldForCleanupAsync(int bookingId, string lockToken, DateTime expiry)
        {
            var memberValue = $"{bookingId}|{lockToken}";

            // Score is the Unix timestamp (seconds since epoch) of the expiration time
            var expiryScore = new DateTimeOffset(expiry).ToUnixTimeSeconds();

            return GetDatabase().SortedSetAddAsync(HOLD_CLEANUP_ZSET, memberValue, expiryScore);
        }

        public Task<RedisValue[]> GetExpiredHoldsAsync()
        {
            var nowScore = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            return GetDatabase().SortedSetRangeByScoreAsync(HOLD_CLEANUP_ZSET, 0, nowScore);
        }
        public Task<bool> RemoveHoldFromCleanupAsync(string memberValue)
        {
            return GetDatabase().SortedSetRemoveAsync(HOLD_CLEANUP_ZSET, memberValue);
        }
    }
}