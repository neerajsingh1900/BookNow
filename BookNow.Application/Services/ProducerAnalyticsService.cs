using BookNow.Application.DTOs.Analytics;
using BookNow.Application.Interfaces;
using BookNow.Application.RepoInterfaces; 
using BookNow.Models; // For Movie
using BookNow.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookNow.Application.Services
{
    public class ProducerAnalyticsService : IProducerAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly ILogger<ProducerAnalyticsService> _logger;

        public ProducerAnalyticsService(
            IUnitOfWork unitOfWork,
            IExchangeRateService exchangeRateService,
            ILogger<ProducerAnalyticsService> logger)
        {
            _unitOfWork = unitOfWork;
            _exchangeRateService = exchangeRateService;
            _logger = logger;
        }

        public async Task<IEnumerable<CountryRevenueDto>> GetRevenueByCountryAsync(int movieId, string targetCurrency)
        {
            // 1. Get current exchange rates (FAST: from Redis cache)
            var rates = await _exchangeRateService.GetRatesAsync();
         
            if (rates == null || !rates.Any())
            {
                _logger.LogError("Currency rates are unavailable. Cannot generate report.");
                return Enumerable.Empty<CountryRevenueDto>();
            }

            // 2. Get raw aggregated data (FAST: from Stored Procedure)
            var rawData = await _unitOfWork.Movie.GetMovieRevenueRawData(movieId);
        
            _logger.LogInformation("Raw revenue data fetched for MovieId {MovieId}: {@RawData}", movieId, rawData);
            if (!rawData.Any())
            {
                return Enumerable.Empty<CountryRevenueDto>();
            }

            // 3. Perform In-Memory Conversion and Final Grouping
            var convertedData = rawData
                .GroupBy(r => new { r.CountryName, r.CountryCode })
                .Select(g =>
                {
                    // Sum all raw amounts after converting each currency group to the target currency
                    decimal totalConvertedRevenue = g.Sum(r =>
                        _exchangeRateService.Convert(
                            r.TotalRawAmount,
                           r.TransactionCurrency,
                            targetCurrency,
                            rates)
                    );

                    return new CountryRevenueDto
                    {
                        CountryName = g.Key.CountryName,
                        CountryCode = g.Key.CountryCode,
                        TotalRevenue = totalConvertedRevenue
                    };
                })
                .OrderByDescending(d => d.TotalRevenue)
                .ToList();
            _logger.LogInformation("converted final:{@convertedData}", convertedData);
            foreach (var d in convertedData)
            {
                _logger.LogInformation("Country: {Country} | Revenue: {Revenue}", d.CountryName, d.TotalRevenue);
            }
            return convertedData;
        }

        // Helper to populate movie list for the view's dropdown
        public async Task<ProducerAnalyticsInputDto> GetInputDataAsync(string producerUserId)
        {
            var todayDateOnly = DateOnly.FromDateTime(DateTime.Today);
          //  m.ReleaseDate <= todayDateOnly &&
           var availableMovies = await _unitOfWork.Movie.GetAllAsync(
     filter: m =>m.ProducerId == producerUserId,   
     orderBy: q => q.OrderByDescending(m => m.ReleaseDate)
 );

            return new ProducerAnalyticsInputDto
            {
                AvailableMovies = availableMovies.ToList()
            };
        }
    }
}