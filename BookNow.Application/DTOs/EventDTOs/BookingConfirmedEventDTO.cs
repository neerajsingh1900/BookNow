using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.EventDTOs
{
    public class BookingConfirmedEventDTO
    {
        public int BookingId { get; set; }
        public string UserEmail { get; set; } = null!;
        public string MovieTitle { get; set; } = null!;
        public DateTime ShowTime { get; set; }
        public decimal TotalAmount { get; set; }
        public string CurrencySymbol { get; set; } = null!;
       
    }
}
