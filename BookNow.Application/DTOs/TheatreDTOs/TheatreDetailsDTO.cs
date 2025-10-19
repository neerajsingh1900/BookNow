using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.TheatreDTOs
{
    public class TheatreDetailDTO
    {
        public int TheatreId { get; set; }
        public string TheatreName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string CityName { get; set; } = null!;

        public int CityId { get; set; }
        public string CountryName { get; set; } = null!;
       
        public string Status { get; set; } = null!;
        public string OwnerId { get; set; } = null!;
        public int ScreenCount { get; set; }

        public string PhoneNumber { get; set; } = null!; 
        public string Email { get; set; } = null!;
    }
}