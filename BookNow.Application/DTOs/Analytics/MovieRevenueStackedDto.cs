using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.Analytics
{
    public class MovieRevenueStackedDto
    {
        public string MovieTitle { get; set; }
        public int MovieId { get; set; }
        public List<CountryContributionDto> RevenueBreakdown { get; set; } = new List<CountryContributionDto>();
    }
}
