
namespace BookNow.Application.DTOs.Analytics
{
   
    public class RawRevenueDto
    {
        public string CountryName { get; set; }
        public string CountryCode { get; set; }

        // Aggregated sum of payment amounts in the original transaction currency
        public decimal TotalRawAmount { get; set; }

        // The original currency code (e.g., "IND", "USD")
        public string TransactionCurrency { get; set; }
    }
}