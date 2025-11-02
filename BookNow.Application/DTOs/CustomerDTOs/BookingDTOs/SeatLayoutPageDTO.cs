using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.CustomerDTOs.BookingDTOs
{
    public class SeatLayoutPageDTO
    {
        public int ShowId { get; set; }
        public string MovieTitle { get; set; }
        public string TheatreName { get; set; }
        public string ScreenName { get; set; }
        public DateTime StartTime { get; set; }
        public string MovieLanguage { get; set; }
        public string CurrencySymbol { get; set; }

        public Dictionary<string, List<SeatStatusDTO>> SeatsByRow { get; set; } = new Dictionary<string, List<SeatStatusDTO>>();
    }
}
