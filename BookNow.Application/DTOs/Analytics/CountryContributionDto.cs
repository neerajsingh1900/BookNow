using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.Analytics
{
    public class CountryContributionDto
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public decimal Revenue { get; set; } 
    }
}
