using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.TheatreDTOs
{
    public class TheatreDetailsDTO
    {
        public int TheatreId { get; set; }
        public string TheatreName { get; set; } = null!;
        public string CityName { get; set; } = null!;
        public string CountryName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Status { get; set; } = null!;

        // Include screens for display on the theatre management page
        public List<ScreenDetailsDTO> Screens { get; set; } = new List<ScreenDetailsDTO>();
    }

    public class ScreenDetailsDTO
    {
        public int ScreenId { get; set; }
        public string ScreenNumber { get; set; } = null!;
        public int TotalSeats { get; set; }
        public decimal DefaultSeatPrice { get; set; }
    }
}