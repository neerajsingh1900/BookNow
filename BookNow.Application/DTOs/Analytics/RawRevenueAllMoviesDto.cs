
namespace BookNow.Application.DTOs.Analytics
{

    public class RawRevenueAllMoviesDto
    {
        
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = null!;


        public string CountryName { get; set; } = null!;
        public string CountryCode { get; set; } = null!;

        public decimal TotalRawAmount { get; set; }

        public string TransactionCurrency { get; set; } = null!;
    }
}