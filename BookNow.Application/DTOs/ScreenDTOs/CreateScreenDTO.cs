using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BookNow.Application.DTOs.ScreenDTOs
{
    public class CreateScreenDTO
    {
        public string ScreenNumber { get; set; } = null!;
        public int NumberOfRows { get; set; }
        public int SeatsPerRow { get; set; }

        public decimal DefaultSeatPrice { get; set; }
        
        public int TheatreId { get; set; }
    }
}