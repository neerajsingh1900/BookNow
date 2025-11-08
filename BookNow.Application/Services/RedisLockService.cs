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

        // Implements SET {key} {value} NX EX {expiry}
        public Task<bool> AcquireLockAsync(string key, string value, TimeSpan expiry)
        {
            // SetIfNx is the Redis SET NX (Not Exists) command
            return GetDatabase().StringSetAsync(key, value, expiry, When.NotExists);
        }

        // Implements Lua script equivalent for atomic DEL if value matches
        public async Task<bool> ReleaseLockAsync(string key, string value)
        {
            // This Lua script is used to ensure the key is only deleted if the value matches, 
            // preventing one user from accidentally releasing another user's lock.
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

            // 2. Convert the RedisResult (which will be 1 or 0) to a boolean
            // (long)result converts the RedisResult to the long integer type that Lua returned.
            return (long)result == 1;
        }
    }
}