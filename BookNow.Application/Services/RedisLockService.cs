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
    }
}