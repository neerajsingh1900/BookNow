using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BookNow.Application.DTOs.TheatreDTOs
{
    public class CreateScreenDTO
    {
        public string ScreenNumber { get; set; } = null!;
        public int NumberOfRows { get; set; }
        public int SeatsPerRow { get; set; }

        public decimal DefaultSeatPrice { get; set; }
        // For the service to know which theatre to attach the screen to
        public int TheatreId { get; set; }
    }
}