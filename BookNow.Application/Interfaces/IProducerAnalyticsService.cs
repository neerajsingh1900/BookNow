using BookNow.Application.DTOs.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.Interfaces
{
    public interface IProducerAnalyticsService
    {
        Task<IEnumerable<MovieRevenueStackedDto>> GetStackedRevenueDataAsync(string producerUserId, string targetCurrency);
    }
}
