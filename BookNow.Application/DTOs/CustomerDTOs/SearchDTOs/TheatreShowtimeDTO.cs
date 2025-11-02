using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.CustomerDTOs.SearchDTOs
{
    public class TheatreShowtimeDTO
    {
        public int TheatreId { get; set; }  
        public string TheatreName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public List<ShowtimeDTO> Showtimes { get; set; } = new List<ShowtimeDTO>();
    }
}
