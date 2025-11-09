using BookNow.Application.Interfaces;
using BookNow.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookNow.Application.Services
{
    
    public class RedisExchangeRateService : IExchangeRateService
    {
        private readonly IDatabase _redisDb;
        private readonly ILogger<RedisExchangeRateService> _logger;
        private readonly HttpClient _httpClient;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(24*7);
        private const string FixerApiEndpoint = "latest";
       
        private readonly string _fixerApiKey;
        private readonly IConfiguration _configuration;
        public RedisExchangeRateService(IRedisLockService redisLockService,
                                        ILogger<RedisExchangeRateService> logger,
                                        HttpClient httpClient, IConfiguration configuration)
        {
            // Reuse connection from existing Redis service
            _redisDb = redisLockService.GetDatabase();
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration;
            _fixerApiKey = _configuration["FixerApi:ApiKey"] ?? throw new InvalidOperationException("Fixer API Key not configured.");
        }

        public async Task<Dictionary<string, decimal>> GetRatesAsync()
        {
            // 1. ATTEMPT CACHE READ (Fast Path)
            HashEntry[] hashEntries = await _redisDb.HashGetAllAsync(IExchangeRateService.CurrencyRatesHashKey);

            if (hashEntries.Length > 0 && hashEntries.Any(e => e.Name.HasValue))
            {
                _logger.LogDebug("Cache hit for exchange rates.");
              return hashEntries.ToDictionary(h => h.Name.ToString()!, h => (decimal)h.Value);
            }

            // 2. CACHE MISS / EXPIRED (Slow Path)
            _logger.LogWarning("Cache miss for exchange rates. Calling external API...");
            var rates = await FetchRatesFromExternalApiAsync();
            // 3. WRITE TO CACHE
            if (rates != null && rates.Any())
            {
                // Set the rates and the 24-hour TTL
                await SetRatesAsync(rates);
            }
            return rates ?? new Dictionary<string, decimal>();
        }
        public decimal Convert(decimal amount, string sourceCurrencyRaw, string targetCurrencyRaw, Dictionary<string, decimal> rates)
        {
            string sourceCurrency = sourceCurrencyRaw;
            string targetCurrency = targetCurrencyRaw;

            if (string.IsNullOrWhiteSpace(sourceCurrency) || string.IsNullOrWhiteSpace(targetCurrency))
            {
                _logger.LogWarning("Invalid currency code(s): Source={Source}, Target={Target}", sourceCurrencyRaw, targetCurrencyRaw);
                return amount;
            }

           

            if (sourceCurrency.Equals(targetCurrency, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("No conversion needed: {Amount} {Currency}", amount, sourceCurrency);
                return amount;
            }

            if (!rates.TryGetValue(sourceCurrency, out var rateSource))
            {
                _logger.LogError("Source currency rate not found: {SourceCurrency}", sourceCurrency);
                return amount;
            }

            if (!rates.TryGetValue(targetCurrency, out var rateTarget))
            {
                _logger.LogError("Target currency rate not found: {TargetCurrency}", targetCurrency);
                return amount;
            }

   
            decimal converted = (amount / rateSource) * rateTarget;
            return Math.Round(converted, 2);
        }



        public async Task SetRatesAsync(Dictionary<string, decimal> rates)
        {
            if (rates == null || !rates.Any()) return;

            var hashEntries = rates
         .Select(kvp => new HashEntry(
             kvp.Key,
            
             (double)kvp.Value
         ))
         .ToArray();

            
            await _redisDb.HashSetAsync(IExchangeRateService.CurrencyRatesHashKey, hashEntries);
            await _redisDb.KeyExpireAsync(IExchangeRateService.CurrencyRatesHashKey, _cacheDuration);
        }

        private async Task<Dictionary<string, decimal>?> FetchRatesFromExternalApiAsync()
        {
            // Implementation specific to the Fixer API structure
            string relativeUrlWithQuery =
                 $"{FixerApiEndpoint}?access_key={_fixerApiKey}";
            try
            {
                var response = await _httpClient.GetAsync(relativeUrlWithQuery);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();

                
                using (JsonDocument document = JsonDocument.Parse(jsonString))
                {
                    if (document.RootElement.TryGetProperty("rates", out JsonElement ratesElement) && ratesElement.ValueKind == JsonValueKind.Object)
                    {
                        var rates = new Dictionary<string, decimal>();
                        foreach (var prop in ratesElement.EnumerateObject())
                        {
                            if (prop.Value.ValueKind == JsonValueKind.Number)
                            {
                                rates.Add(prop.Name, prop.Value.GetDecimal());
                            }
                        }
                      
                        rates[IExchangeRateService.BaseCurrency] = 1.0M;
                        return rates;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching exchange rates from external API.");
            }
            return null;
        }
    }
}