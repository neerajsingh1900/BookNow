using BookNow.Models;
using System.Collections.Generic;
using System.Linq;

namespace BookNow.Application.DTOs.Analytics
{
    public class ProducerAnalyticsInputDto
    {
        // List of movies available for selection (assuming Movie is a Model entity)
        public IEnumerable<Movie> AvailableMovies { get; set; }

        // Final report data, populated only after the report is run
        public ICollection<CountryRevenueDto> ReportData { get; set; } = new List<CountryRevenueDto>();

        // A static list of global currencies for the target currency dropdown
        public List<string> AvailableCurrencies => new List<string> {
            "USD", "EUR", "GBP", "INR", "CAD", "AUD", "JPY", "SGD"
        };
    }
}