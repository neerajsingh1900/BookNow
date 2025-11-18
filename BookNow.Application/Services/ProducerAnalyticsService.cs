using BookNow.Application.DTOs.Analytics;
using BookNow.Application.Interfaces;
using BookNow.Application.RepoInterfaces; 
using BookNow.Models;
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

        public async Task<IEnumerable<MovieRevenueStackedDto>> GetStackedRevenueDataAsync(string producerUserId, string targetCurrency)
        {
           
            var rates = await _exchangeRateService.GetRatesAsync();

            if (rates == null || !rates.Any())
            {
                _logger.LogError("Currency rates are unavailable. Cannot generate stacked report.");
                return Enumerable.Empty<MovieRevenueStackedDto>();
            }

            var rawData = await _unitOfWork.Movie.GetProducerMoviesRevenueRawData(producerUserId);

            if (!rawData.Any())
            {
                return Enumerable.Empty<MovieRevenueStackedDto>();
            }

            var convertedData = rawData
                .Select(r => new
                {
                    r.MovieId,
                    r.MovieTitle,
                    r.CountryCode,
                    r.CountryName,
                    ConvertedRevenue = _exchangeRateService.Convert(
                        r.TotalRawAmount,
                        r.TransactionCurrency,
                        targetCurrency,
                        rates
                    )
                })
                .GroupBy(r => new { r.MovieId, r.MovieTitle, r.CountryCode, r.CountryName })
                .Select(g => new
                {
                    g.Key.MovieId,
                    g.Key.MovieTitle,
                    g.Key.CountryCode,
                    g.Key.CountryName,
                    TotalConvertedRevenue = g.Sum(x => x.ConvertedRevenue)
                })
                .ToList();

            var finalData = convertedData
               .GroupBy(d => new { d.MovieId, d.MovieTitle })
                .OrderBy(g => g.Key.MovieId) 
                .Select(g => new MovieRevenueStackedDto
                {
                    MovieId = g.Key.MovieId,
                    MovieTitle = g.Key.MovieTitle,

                    RevenueBreakdown = g.Select(c => new CountryContributionDto
                    {
                        CountryCode = c.CountryCode,
                        CountryName = c.CountryName,
                        Revenue = c.TotalConvertedRevenue
                    })
                    .OrderByDescending(c => c.Revenue)
                    .ToList()
                })
                .ToList();
               
            return finalData;
        }

    }
}