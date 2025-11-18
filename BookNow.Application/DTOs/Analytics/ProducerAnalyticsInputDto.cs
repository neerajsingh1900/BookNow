using BookNow.Models;
using System.Collections.Generic;
using System.Linq;

namespace BookNow.Application.DTOs.Analytics
{
    public class ProducerAnalyticsInputDto
    {
        public List<string> AvailableCurrencies => new List<string> {
            "USD", "EUR", "GBP", "INR", "CAD", "AUD", "JPY", "SGD"
        };
    }
}