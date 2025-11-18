using BookNow.Application.Interfaces;
using BookNow.Application.DTOs.PaymentDTOs;
using Hangfire;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace BookNow.Application.Services.Cleanup
{
    
    public class ExpiredHoldCleanupLogic
    {
        private readonly IRedisLockService _redisLockService;
        private readonly IPaymentService _paymentService; 
        private readonly ILogger<ExpiredHoldCleanupLogic> _logger;

        public ExpiredHoldCleanupLogic(
            IRedisLockService redisLockService,
            IPaymentService paymentService,
            ILogger<ExpiredHoldCleanupLogic> logger)
        {
            _redisLockService = redisLockService;
            _paymentService = paymentService;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task CleanupExpiredHoldsAsync()
        {
            RedisValue[] expiredHolds = await _redisLockService.GetExpiredHoldsAsync();

            if (expiredHolds.Length == 0) return;
                
            _logger.LogInformation("Found {Count} expired holds in Redis ZSET for cleanup.", expiredHolds.Length);

            foreach (var holdMember in expiredHolds)
            {
                if (!holdMember.HasValue) continue;

                string memberString = holdMember.ToString();

                var parts = memberString.Split('|');

                if (parts.Length != 2 || !int.TryParse(parts[0], out int bookingId))
                {
                    _logger.LogError("Malformed Redis ZSET member: {Member}. Removing.", holdMember);
                    await _redisLockService.RemoveHoldFromCleanupAsync(holdMember.ToString());
                    continue;
                }

                string lockToken = parts[1];

                try
                {
                    var timeoutResponse = new GatewayResponseDTO
                    {
                        BookingId = bookingId,
                        Status = "timeout", 
                        IdempotencyKey = lockToken
                    };

                    await _paymentService.ProcessGatewayResponseAsync(timeoutResponse);

                    await _redisLockService.RemoveHoldFromCleanupAsync(holdMember.ToString());
                }
                catch (Exception ex)
                {
                     _logger.LogError(ex, "Failed to cleanup expired hold for BookingId {BookingId}.", bookingId);
                }
            }
            _logger.LogInformation("Hangfire cleanup job finished.");
        }
    }
}