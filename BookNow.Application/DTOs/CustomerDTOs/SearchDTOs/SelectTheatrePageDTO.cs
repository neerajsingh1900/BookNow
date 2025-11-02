using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.CustomerDTOs.SearchDTOs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SelectTheatrePageDTO
    {
       
        public MovieListingDTO Movie { get; set; } = null!;

        public List<TheatreShowtimeDTO> Theatres { get; set; } = new List<TheatreShowtimeDTO>();

        public DateOnly ActiveDate { get; set; }
        public List<DateOnly> FixedDateWindow { get; set; } = new List<DateOnly>();
        public List<DateOnly> AvailableDates { get; set; } = new List<DateOnly>();
        public bool IsShowtimeAvailableInWindow => AvailableDates.Any();
    }

}
