
using BookNow.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookNow.Application.Services.BackgroundTasks
{
    public class RateCacheWarmerJob : BackgroundService
    {
        // Define how often the job attempts to run (e.g., every 6 hours)
        private static readonly TimeSpan ExecutionInterval = TimeSpan.FromDays(7);

        private readonly IExchangeRateService _exchangeRateService;
        private readonly ILogger<RateCacheWarmerJob> _logger;

        public RateCacheWarmerJob(IExchangeRateService exchangeRateService,
                                  ILogger<RateCacheWarmerJob> logger)
        {
            _exchangeRateService = exchangeRateService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Rate Cache Warmer Job started.");

            // Wait for a short period before the first execution to ensure app is fully ready
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Rate Cache Warmer is running at: {Time}", DateTime.Now);

                try
                {
                    // The job calls the service, which executes the "Slow Path" logic:
                    // 1. Checks Redis (Cache Miss because TTL has expired or we're forcing a refresh).
                    // 2. Calls External API (Fixer).
                    // 3. Writes fresh rates back to Redis with a new 24h TTL.
                    var rates = await _exchangeRateService.GetRatesAsync();

                    if (rates != null && rates.Count > 0)
                    {
                        _logger.LogInformation("Rate cache successfully warmed with {Count} currencies.", rates.Count);
                    }
                    else
                    {
                        _logger.LogError("Rate cache warming failed: No rates returned from service.");
                    }
                }
                catch (Exception ex)
                {
                    // Log the error but DO NOT re-throw, allowing the loop to continue
                    _logger.LogError(ex, "An error occurred during Rate Cache Warmer execution.");
                }

                // Wait for the defined interval before running again
                await Task.Delay(ExecutionInterval, stoppingToken);
            }

            _logger.LogInformation("Rate Cache Warmer Job stopped.");
        }
    }
}