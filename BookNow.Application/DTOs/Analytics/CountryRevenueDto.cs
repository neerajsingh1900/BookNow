
namespace BookNow.Application.DTOs.Analytics
{
    
    public class CountryRevenueDto
    {
        public string CountryName { get; set; }
        public string CountryCode { get; set; }

        
        public decimal TotalRevenue { get; set; }
    }
}