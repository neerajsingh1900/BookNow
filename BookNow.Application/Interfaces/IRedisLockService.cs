using StackExchange.Redis;

namespace BookNow.Application.Interfaces
{
    public interface IRedisLockService
    {
      
        Task<bool> AcquireLockAsync(string key, string value, TimeSpan expiry);

        // Value must match the lock holder to ensure only the owner releases the lock
        Task<bool> ReleaseLockAsync(string key, string value);

        // Gets the IDatabase instance for direct operations (like Pub/Sub)
        IDatabase GetDatabase();
    }
}