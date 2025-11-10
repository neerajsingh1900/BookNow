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

             await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Rate Cache Warmer is running at: {Time}", DateTime.Now);

                try
                {
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
                     _logger.LogError(ex, "An error occurred during Rate Cache Warmer execution.");
                }

                 await Task.Delay(ExecutionInterval, stoppingToken);
            }

            _logger.LogInformation("Rate Cache Warmer Job stopped.");
        }
    }
}