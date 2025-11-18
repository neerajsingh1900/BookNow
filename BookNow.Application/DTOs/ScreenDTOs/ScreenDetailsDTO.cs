using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.ScreenDTOs
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace BookNow.Application.DTOs.ScreenDTOs
    {
       
        public class ScreenDetailsDTO
        {
            public int ScreenId { get; set; }
            public int TheatreId { get; set; }
            public string ScreenNumber { get; set; } = null!;
            public int TotalSeats { get; set; }

            [Column(TypeName = "decimal(10, 2)")]
            public decimal DefaultSeatPrice { get; set; }

            public int CurrentShowCount { get; set; }

            public int NumberOfRows { get; set; }
            public int SeatsPerRow { get; set; }

            public bool HasActiveBookings { get; set; }
        }
    }

}
